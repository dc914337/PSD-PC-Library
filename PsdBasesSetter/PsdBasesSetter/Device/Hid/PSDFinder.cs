using System.Configuration;
using System.Linq;
using HidSharp;
using PsdBasesSetter.Config;
using System;

namespace PsdBasesSetter.Device.Hid
{
    public class PSDFinder
    {
        private PSDConfigSection _configSection;

        HidDeviceLoader _loader = new HidDeviceLoader();
        public PSDFinder()
        {
             _configSection = ConfigurationManager.OpenExeConfiguration(
                 ConfigurationUserLevel.None).GetSection("PSDConfigSection") as PSDConfigSection;

            //if we couldnt find config we take config from res
            if(_configSection==null)
            {
                _configSection = new PSDConfigSection();
                var str2 = Properties.Resources.PID.Substring(2);
                _configSection.PID = Convert.ToInt32(str2, 16);
                _configSection.VID = Convert.ToInt32(Properties.Resources.VID.Substring(2), 16);
                _configSection.InputReportLength = Convert.ToInt32(Properties.Resources.PSDInputReportLength);
            }
        }

        public PSDDevice[] FindConnectedPsds()
        {
            var psds = _loader.GetDevices().Where(
                    d => d.MaxInputReportLength == _configSection.InputReportLength &&
                    d.VendorID == _configSection.VID &&
                    d.ProductID == _configSection.PID);

            return psds.Select(a => new PSDDevice(a)).ToArray();
        }
    }
}
