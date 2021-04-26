using System;
using Autofac;
using Autofac.Core;

namespace AutoFacCourse
{
    internal class Program
    {
        public interface ILog
        {
            void Write(string message);
        }

        public class ConsoleLog : ILog
        {
            public void Write(string message)
            {
                Console.WriteLine(message);
            }
        }

        public class EmailLog : ILog
        {
            private const string adminEmail = "admin@foo.com";


            public void Write(string message)
            {
                Console.WriteLine($"Email sent to {adminEmail} : {message}");
            }
        }

        public class SmsLog : ILog
        {

            public string phoneNumber;

            public SmsLog(string phoneNumber)
            {
                this.phoneNumber = phoneNumber;
            }

            public void Write(string message)
            {
                Console.WriteLine($"SMS to {phoneNumber} : {message}");
            }
        }

        public class Engine
        {
            private ILog log;
            private int id;

            public Engine(ILog log)
            {
                this.log = log;
                id = new Random().Next();
            }
            public Engine(ILog log, int engineId)
            {
                this.log = log;
                id = engineId;
            }

            public void Ahead(int power)
            {
                log.Write($"Engine [{id}] ahead {power}");
            }

        }

        public class Car
        {
            private ILog log;
            private Engine engine;

            public Car(Engine engine, ILog log)
            {
                this.log = log;
                this.engine = engine;
            }

            public Car(Engine engine)
            {
                this.log = new EmailLog();
                this.engine = engine;
            }


            public void Go()
            {
                engine.Ahead(100);
                log.Write("Car going forward...");
            }

        }


        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            // named parameter
            //builder.RegisterType<SmsLog>().As<ILog>()
            //    .WithParameter("phoneNumber", "8173804286");

            // typed parameter
            //builder.RegisterType<SmsLog>().As<ILog>()
            //    .WithParameter(new TypedParameter(typeof(string), "8173804286"));
             
            // typed parameter
            //builder.RegisterType<SmsLog>().As<ILog>()
            //    .WithParameter(
            //        new ResolvedParameter(
            //            // predicate
            //          (pi, ctx) => pi.ParameterType == typeof(string) && pi.Name == "phoneNumber",
            //            // value accessor
            //            (pi, ctx) => "8173804286"
            //            )
            //        );


            Random random = new Random();
            builder.Register((c, p) => new SmsLog(p.Named<string>("phoneNumber")))
                .As<ILog>();

            IContainer container = builder.Build();

            var log = container.Resolve<ILog>(new NamedParameter("phoneNumber",  random.Next().ToString()));

            log.Write("Test message.");
        }
    }
}
