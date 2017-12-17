using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESPBackend.Dto
{
    public class PingResponseDto
    {
        public string ServiceVersion { get; set; }
        public TimeSpan Uptime { get; set; }

        public override string ToString()
        {
            return $"{nameof(ServiceVersion)}: {ServiceVersion}, {nameof(Uptime)}: {Uptime}";
        }
    }
}
