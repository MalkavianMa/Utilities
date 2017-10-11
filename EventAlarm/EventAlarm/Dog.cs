using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventAlarm
{
    class Dog
    {
        //1.声明关于事件的委托
        public delegate void AlarmEventHandler(object sender, EventArgs e);
        //2.声明事件;
        public event AlarmEventHandler Alarm;

        //3.编写引发事件的函数
        public void OnAlarm()
        {
            if (this.Alarm!=null)
            {
                Console.WriteLine("\n狗报警: 有小偷进来了,旺旺!!!!!");
                this.Alarm(this, new EventArgs());//发出警报
            }


        }

    }
}
