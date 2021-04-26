using System;
using Autofac;

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
            builder.RegisterType<ConsoleLog>().As<ILog>();

            builder.Register((IComponentContext c) => 
                new Engine(c.Resolve<ILog>(), 123));
            builder.RegisterType<Car>();

            IContainer container = builder.Build();

            var car = container.Resolve<Car>();
            car.Go();
        }
    }
}
