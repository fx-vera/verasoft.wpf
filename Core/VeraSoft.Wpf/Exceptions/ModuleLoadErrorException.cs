using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;

namespace VeraSoft.Wpf.Exceptions
{
    /// <summary>
    /// Makes easier getting the actual error from a CompositionException
    /// </summary>
    public class ModuleLoadErrorException : Exception
    {
        public ModuleLoadErrorException()
        { }

        public ModuleLoadErrorException(string err) : base(err)
        { }

        public ModuleLoadErrorException(string err, Exception inner) : base(err, inner)
        { }

        public static ModuleLoadErrorException CreateFromCompositionException(CompositionException ce)
        {
            Exception unwrappedException = UnwrapCompositionException(ce);
            Exception validException = unwrappedException ?? ce;
            StringBuilder sbMessage = new StringBuilder("Error loading modules with MEF: ");
            sbMessage.Append(validException.Message);
            int errNum = 1;
            foreach (CompositionError err in ce.Errors)
            {
                sbMessage.AppendFormat("\r\nError {0}/{1}", errNum, ce.Errors.Count)
                         .Append(err.Description).Append(" | ")
                         .Append(err.Element.DisplayName).Append(" | ")
                         .Append(err.Exception.Message);
                errNum++;
            }

            return new ModuleLoadErrorException(sbMessage.ToString(), validException);
        }

        /// <summary>
        /// Attempts to retrieve the real cause of a composition failure.
        /// </summary>
        /// <remarks>
        /// https://haacked.com/archive/2014/12/09/unwrap-mef-exception/
        /// Sometimes a MEF composition fails because an exception occurs in the ctor of a type we're trying to
        /// create. Unfortunately, the CompositionException doesn't make that easily available, so we don't get
        /// that info in haystack. This method tries to find that exception as that's really the only one we care
        /// about if it exists. If it can't find it, it returns the original composition exception.
        /// </remarks>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static Exception UnwrapCompositionException(CompositionException exception)
        {
            //var compositionException = exception as CompositionException;
            //if (compositionException == null)
            //{
            //    return exception;
            //}

            var unwrapped = exception;// compositionException;
            while (unwrapped != null)
            {
                var firstError = unwrapped.Errors?.FirstOrDefault();
                if (firstError == null)
                {
                    break;
                }
                var currentException = firstError.Exception;

                if (currentException == null)
                {
                    break;
                }

                var composablePartException = currentException as ComposablePartException;

                if (composablePartException != null
                    && composablePartException.InnerException != null)
                {
                    var innerCompositionException = composablePartException.InnerException as CompositionException;
                    if (innerCompositionException == null)
                    {
                        return currentException.InnerException ?? exception;
                    }
                    currentException = innerCompositionException;
                }

                unwrapped = currentException as CompositionException;
            }

            return exception; //Couldn't find the real deal. Throw the original.
        }
    }
}
