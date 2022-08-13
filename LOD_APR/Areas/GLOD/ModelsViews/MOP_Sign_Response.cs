using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.GLOD.ModelsViews
{
    public class MOP_Post_Sign
    {
        public string api_token_key { get; set; }
        public string token { get; set; }
        public string otp { get; set; }
        public List<File> files { get; set; }
    }

    public class File
    {
        [DisplayName("content-type")]
        public string contentType { get; set; }
        public string content { get; set; }
        public string description { get; set; }
        public string checksum { get; set; }
        public string status { get; set; }
        public string documentStatus { get; set; }
        public string checksum_original { get; set; }
    }

    public class MOP_Post_Response
    {
        public File[] files { get; set; }
        public Metadata metadata { get; set; }
        public long idSolicitud { get; set; }
        public string error { get; set; }
    }

    public class Metadata
    {
        public bool otpExpired { get; set; }
        public int filesSigned { get; set; }
        public int signedFailed { get; set; }
        public int objectsReceived { get; set; }
    }
}