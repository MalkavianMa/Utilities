using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
 namespace elanguage.Util 
 {
	public class Debug
    {

        public static void Debugwrite(Object o)
        {
            Debugwrite(o, System.Console.Out);
            Console.ReadKey();
		}

        public static void Debugwrite(Object o, TextWriter xout)
        {
	
			if (o  is  String) {
				xout.Write(o);
			} else if (o  is  Object[]) {
	
				Object[] arr = (Object[]) o;
				if (arr != null && arr.Length > 0) {
					for (int i = 0; i < arr.Length; i++) {
						if (i > 0) {
							System.Console.Out.Write(",");
						}
                        Debugwrite(arr[i], xout);
					}
					xout.WriteLine();
				}
	
			} else if (o  is  IList) {
				IList list = (IList) o;
				if (list != null && list.Count > 0) {
					for (int i_0 = 0; i_0 < list.Count; i_0++) {
                        Debugwrite(list[i_0], xout);
					}
	
				}
			} else {
                Debugwrite(o.ToString());
			}
		}
	}
}
