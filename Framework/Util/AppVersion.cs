using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
	public struct AppVersion
	{
		public enum Stage
		{
			INDEV,
			ALPHA,
			BETA,
			RELEASE
		}

		public Stage stage;
		public int major;
		public int minor;
        public int fix;

		public AppVersion(Stage stage, int major, int minor=0, int fix=0)
		{
			this.stage = stage;
			this.major = major;
			this.minor = minor;
            this.fix = fix;
		}

		public override string ToString()
		{
			string stageStr="";
			switch(stage)
			{
				case Stage.INDEV: stageStr = "Indev "; break;
				case Stage.ALPHA: stageStr = "Alpha "; break;
				case Stage.BETA: stageStr = "Beta "; break;
				case Stage.RELEASE: stageStr = ""; break;
				default: break;
			}
            string str = stageStr + major + "." + minor;
            if(fix != 0)
                str += "." + fix;
			return str;
		}
	}
}


