using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//要创建一个事件驱动的程序需要下面的步骤：

//1.         声明关于事件的委托；

//2.         声明事件；

//3.         编写触发事件的函数；

//4.         创建事件处理程序；

//5.         注册事件处理程序；

//6.         在适当的条件下触发事件。

//现在我们来编写一个自定义事件的程序。主人养了一条忠实的看门狗，晚上主人睡觉的时候，狗负责看守房子。一旦有小偷进来，狗就发出一个Alarm事件，主人接到Alarm事件后就会采取相应的行动。假设小偷于2014
namespace EventAlarm
{
    class Program
    {
        static void Main(string[] args)
        {
            Dog dog = new Dog();
            Host host = new Host(dog);
            //当前时间 从2017-10-11 10:47:58开始计时
            DateTime now = new DateTime(2017, 10, 11, 10, 50,50);
            DateTime midNight = new DateTime(2017, 10, 11, 10, 59, 50);

            //等待午夜的到来
            Console.WriteLine("时间在里哭时");
            while       (now<midNight)
            {
                Console.WriteLine("当前时间"+now);

                System.Threading.Thread.Sleep(1000);//程序暂停一秒
                now = now.AddSeconds(1);//时间增加一毛
            }

            //午夜零点小偷到达，看门狗引发Alarm事件
            Console.WriteLine("\n月黑风高的午夜"+now);
            Console.WriteLine("小偷悄悄地摸进了主人的屋内>>");
            dog.OnAlarm();
            Console.ReadLine();


        }
    }
}
