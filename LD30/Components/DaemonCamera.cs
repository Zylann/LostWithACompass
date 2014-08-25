using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD30.Components
{
    public class DaemonCamera : Behaviour
    {
        private int state = 0;

        public override void OnCreate()
        {
            base.OnCreate();
            entity.AddComponent<Camera>().AddLayer(ViewLayers.DAEMON_OBJECTS);
            entity.camera.enableLighting = false;
        }

        public void Capture()
        {
            state = 1;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if(state == 1)
            {
                entity.camera.AddLayer(ViewLayers.DAEMON_OBJECTS);
                state = 2;
            }
            else if(state == 2)
            {
                entity.camera.RemoveLayer(ViewLayers.DAEMON_OBJECTS);
                state = 0;
            }
        }
    }
}
