using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace VeraSoft.Wpf.Exceptions
{
    public class UnhandledExceptionHandler
    {
        /// <summary>
        /// Constructor estático, para poder inicializar los handlers de excepciones sin crear ninguna
        /// instancia de esta clase
        /// Ver: http://msdn.microsoft.com/es-es/library/k9x6w0hc.aspx
        /// Según la documentación: 
        /// "Es llamado automáticamente antes de crear la primera instancia o de hacer referencia a cualquier miembro estático."
        /// </summary>
        static UnhandledExceptionHandler()
        {
            // Add the event handler for handling UI thread exceptions to the event.
            System.Windows.Forms.Application.ThreadException += new ThreadExceptionEventHandler(ClienteBasico_UIThreadException);

            // Set the unhandled exception mode to force all Windows Forms errors to go through 
            // our handler.
            System.Windows.Forms.Application.SetUnhandledExceptionMode(System.Windows.Forms.UnhandledExceptionMode.CatchException);

            // Add the event handler for handling non-UI thread exceptions to the event. 
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            //Eventos de WPF no recogidos
            Application.Current.DispatcherUnhandledException += DispatcherUnhandledException;

            //Y una que no sale: excepciones no capturadas en las Tasks
            System.Threading.Tasks.TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        /// <summary>
        /// Método vacío que sirve sólo para que en algún momento se llame al constructor estático, sin
        /// necesidad de crear ninguna instancia de la clase
        /// </summary>
        public static void Init()
        { }

        static void DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
#if DEBUG
            LogException(e.Exception, true);
#else
            LogException(e.Exception, false);
#endif
        }

        /// <summary>
        /// Función que recoge las excepciones no capturadas de las Task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void TaskScheduler_UnobservedTaskException(object sender, System.Threading.Tasks.UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
#if DEBUG
            LogException(e.Exception, true);
#else
            LogException(e.Exception, false);
#endif
        }


        /// <summary>
        /// Función que recoge las excepciones no capturadas en general de un dominio, no incluye (creo)
        /// las de Tasks o las de UI de WPF o Windows Forms
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e != null)
            {
                LogException(e.ExceptionObject as Exception, !e.IsTerminating);
                try
                {
                    if (e.IsTerminating)
                    {
#if DEBUG
                        MessageBox.Show("Excepción no controlada: " + (e.ExceptionObject as Exception).ToString(),
                                        "Error grave en aplicación",
                                        MessageBoxButton.OK, MessageBoxImage.Stop);
#else
                        Trace.TraceError("Excepción no controlada: " + (e.ExceptionObject as Exception).ToString());
#endif
                    }
                }
                catch (Exception)
                { }
                finally
                {
#if DEBUG
                    Application.Current.Shutdown();
#endif
                }
            }
            else
            {
                LogException(null, true);
            }
        }

        /// <summary>
        /// Función que recoge las excepciones no capturadas de UI de Windows Forms
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="t"></param>
        private static void ClienteBasico_UIThreadException(object sender, ThreadExceptionEventArgs t)
        {
            LogException(t.Exception, true);
        }


        private static void LogException(Exception ex, bool bShowTerminationDialog)
        {
            try
            {
                // Create an EventLog instance and assign its source.
                if (ex != null)
                {
                    Exception temp = ex;
                    string errorMsg = "Cadena de excepciones: \r\n\t" + ex.Message + "\r\n";
                    while (temp.InnerException != null)
                    {
                        errorMsg += "\t" + temp.InnerException.Message + "\r\n";
                        temp = temp.InnerException;
                    }

                    errorMsg += "--------------------------------\r\n";
                    errorMsg += "Fuente: " + ex.Source + "\r\n";
                    errorMsg += "Error completo: " + temp.ToString();

                    Trace.TraceError("\r\n-----------------------------------------------------------------\r\n" +
                                            "---------------  Error no controlado en aplicación  --------------\r\n" +
                                            "-----------------------------------------------------------------\r\n" +
                                            errorMsg + "\r\n" +
                                            "-----------------------------------------------------------------\r\n");
                }
                else
                {
                    Trace.TraceError("\r\n-----------------------------------------------------------------\r\n" +
                                            "----------------- Error no controlado en aplicación  -------------\r\n" +
                                            "-----------------------------------------------------------------\r\n" +
                                            "Excepción es null\r\n" +
                                            "-----------------------------------------------------------------\r\n");
                }

#if DEBUG
                if (bShowTerminationDialog)
                {
                    MessageBoxResult result = MessageBoxResult.None;
                    result = ShowExceptionDialogAndAskToContinue("Excepción grave en aplicación", ex);
                    // Exits the program when the user clicks Abort. 
                    if (result == MessageBoxResult.No)
                        Application.Current.Shutdown();
                }
#endif
            }
            catch (Exception exc)
            {
                try
                {
#if DEBUG
                    MessageBox.Show("Fatal Non-UI Error",
                        "Fatal Non-UI Error. Could not write the error to the event log. Reason: "
                        + exc.Message, MessageBoxButton.OK, MessageBoxImage.Stop);
#endif
                }
                finally
                {
                    Application.Current.Shutdown();
                }
            }
        }


        // Creates the error message and displays it. 
        private static MessageBoxResult ShowExceptionDialogAndAskToContinue(string title, Exception e)
        {
            string errorMsg = "Error no controlado en aplicación. ¿Desea proseguir la ejecución?\r\n\r\n" +
                                "-----------------------------------------------------------------\r\n\r\n";
            if (e != null)
            {
                errorMsg += e.Message;

                Exception temp = e;
                while (temp.InnerException != null)
                {
                    errorMsg += "\t" + temp.InnerException.Message + "\r\n";
                    temp = temp.InnerException;
                }

                errorMsg += "--------------------------------\r\n";
                errorMsg += "Fuente: " + e.Source + "\r\n";
                errorMsg += "Error completo: " + e.ToString();

            }
            else
            {
                errorMsg += "Error desconocido (excepción nula)";
            }

            //Trace.TraceError(errorMsg);
            return MessageBox.Show(errorMsg, title, MessageBoxButton.YesNo, MessageBoxImage.Stop);
        }
    }

}
