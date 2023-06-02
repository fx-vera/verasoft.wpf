namespace VeraSoft.Wpf.Core
{
    //    Single Responsibility Principle(SRP)
    // un método 1 acción/responsabilidad


    //Open Closed Principle(OCP)
    /*
     * 
     * 
     * 
    que se pueda extender o cambiar sin cambiar el core
    We can apply OCP by using interface, abstract class, abstract methods and virtual methods when you want to extend functionalit


        */
    //Liskov Substitution Principle(LSP)
    /*
    Cuando se hereda, poder usar el hijo sin conocer el padre



        */
    //Interface Segregation Principle(ISP)
    /*
    que cada clase que herede tenga lo que necesita, nada más




        */
    //Dependency Inversion Principle(DIP)
    /*
 IoC



    */
    /***************************************************************otro****************************************/
    //    The following data types are all of VALUE TYPE: // cuando pasas el valor por un metodo, no cambia de valor
    // son nullables        // ALLOCATED IN THE STACK
    //bool
    //byte
    //char
    //decimal
    //double
    //enum      // no se puede declarar dentro de un método
    //float
    //int
    //long
    //sbyte
    //short
    //struct        // can include a constructor and methods
    //uint
    //ulong
    //ushort


    //    The followings are REFERENCE TYPE data types: NULL by default
    // ALLOCATED IN THE HEAP
    //String // inmutable, cuando lo pasas por parámetro no cambia de valor el original.
    //Arrays(even if their elements are value types)
    //Class
    //Delegate


    //    float De ±1,5 x 10-45 a ±3,4 x 1038	De 6 a 9 dígitos aproximadamente    4 bytes System.Single
    //double De ±5,0 × 10−324 a ±1,7 × 10308	De 15 a 17 dígitos aproximadamente	8 bytes System.Double
    //decimal De ±1,0 x 10-28 to ±7,9228 x 1028	28-29 dígitos                       16 bytes System.Decimal   monetary value



    //sbyte De -128 a 127	Entero de 8 bits con signo System.SByte
    //byte De 0 a 255	Entero de 8 bits sin signo System.Byte
    //short De -32 768 a 32 767	Entero de 16 bits con signo System.Int16
    //ushort De 0 a 65.535	Entero de 16 bits sin signo System.UInt16
    //int De -2.147.483.648 a 2.147.483.647	Entero de 32 bits con signo System.Int32
    //uint De 0 a 4.294.967.295	Entero de 32 bits sin signo System.UInt32
    //long De -9.223.372.036.854.775.808 a 9.223.372.036.854.775.807	Entero de 64 bits con signo System.Int64
    //ulong De 0 a 18.446.744.073.709.551.615	Entero de 64 bits sin signo System.UInt64
    //nint    Depende de la plataforma    Entero de 64 bits o 32 bits con signo System.IntPtr
    //nuint   Depende de la plataforma    Entero de 64 bits o 32 bits sin signo System.UIntPtr
}