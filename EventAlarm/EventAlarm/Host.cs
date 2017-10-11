using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventAlarm
{        //事件接收者

    class Host
    {//4.编写事件处理程序

        void HostHandleAlarm(object sender,EventArgs e)
        {
            Console.WriteLine("主人:抓住了小偷!!");
        }
        //5.注册事件处理程序
        public Host(Dog dog)
        {

            dog.Alarm += new Dog.AlarmEventHandler(HostHandleAlarm);

        }
    }
}
