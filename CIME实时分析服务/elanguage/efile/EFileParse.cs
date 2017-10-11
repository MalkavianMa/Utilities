using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
 namespace elanguage.Efile 
 {

	public interface EFileParse {
	
		/// <summary>
		/// ����E�����ļ������
		/// </summary>
		///
		/// <param name="path"></param>
		/// <returns></returns>
		/// <exception cref="System.Exception">List<ETable></exception>
		/// @Date Dec 11, 2012
		List<ETable> ParseFile(String path);

        void CreateEFile(ETable etb, string startsWith, string path);
	}
}
