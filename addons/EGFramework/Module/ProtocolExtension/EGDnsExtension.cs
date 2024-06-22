using System;
using System.Collections.Generic;

namespace EGFramework{
    /// <summary>
    /// Support a class for analysis the .local(mdns) or other www.com(dns) protocol to get the message,
    /// mdns protocol format type reference from https://github.com/richardschneider/net-mdns
    /// nuget package is Makaretu.Dns.Multicast
    /// mdns reference from https://www.rfc-editor.org/rfc/rfc6763.html
    /// </summary>
    public static class EGDnsExtension 
    {
        public const string DefaultDnsServer = "1.1.1.1";
        public const int DefaultDnsPort = 53;
        public const string DefaultMDnsServerIpv4 = "224.0.0.251";
        public const string DefaultMDnsServerIpv6 = "FF02::FB";
        public const int DefaultMDnsPort = 5353;

        public const string DNS_SRV_RR = "_services._dns-sd._udp.local";
    }

    /// <summary>
    /// Dns's OpCode
    /// </summary>
    public enum DnsOpCode : ushort{
        /// <summary>
        /// Standard query.
        /// </summary>
        Query = 0x0000,
        /// <summary>
        /// Inverse query (obsolete), see https://tools.ietf.org/html/rfc3425.
        /// </summary>
        InverseQuery = 0x0800,
        /// <summary>
        /// A server status request.
        /// </summary>
        Status = 0x1000,
        /// <summary>
        /// Zone change, see https://tools.ietf.org/html/rfc1996.
        /// </summary>
        Notify = 0x2000,
        /// <summary>
        /// Update message, see https://tools.ietf.org/html/rfc2136.
        /// </summary>
        Update = 0x2800
    }

    /// <summary>
    /// A resource record or query type.
    /// </summary>
    public enum DnsType{
        /// <summary>
        /// A host address.
        /// </summary>
        A = 1,
        /// <summary>
        /// An authoritative name server.
        /// </summary>
        NS = 2,
        /// <summary>
        /// The canonical name for an alias.
        /// </summary>
        CNAME = 5,
        /// <summary>
        /// Marks the start of a zone of authority.
        /// </summary>
        SOA = 6,
        /// <summary>
        /// A mailbox domain name (EXPERIMENTAL).
        /// </summary>
        MB = 7,
        /// <summary>
        /// A mail group member (EXPERIMENTAL).
        /// </summary>
        MG = 8,
        /// <summary>
        /// A mailbox rename domain name (EXPERIMENTAL).
        /// </summary>
        MR = 9,
        /// <summary>
        /// A Null resource record (EXPERIMENTAL).
        /// </summary>
        NULL = 10,
        /// <summary>
        /// A well known service description.
        /// </summary>
        WKS = 11,
        /// <summary>
        /// A domain name pointer.
        /// </summary>
        PTR = 12,
        /// <summary>
        /// Host information.
        /// </summary>
        HINFO = 13,
        /// <summary>
        /// Mailbox or mail list information.
        /// </summary>
        MINFO = 14,
        /// <summary>
        /// Mail exchange.
        /// </summary>
        MX = 15,
        /// <summary>
        /// Text resources.
        /// </summary>   
        TXT = 16,
        /// <summary>
        /// Responsible Person.
        /// </summary>
        RP = 17,
        /// <summary>
        /// AFS Data Base location.
        /// </summary>
        AFSDB = 18,
        /// <summary>
        /// An IPv6 host address.
        /// </summary>
        AAAA = 28,
        /// <summary>
        /// A resource record which specifies the location of the server(s) for a specific protocol and domain.  
        /// </summary>
        SRV = 33,
        /// <summary>
        /// Maps an entire domain name.
        /// </summary>
        DNAME = 39,
        /// <summary>
        /// Option record.
        /// </summary>
        OPT = 41,
        /// <summary>
        /// Delegation Signer.
        /// </summary>
        DS = 43,
        /// <summary>
        /// Signature for a RRSET with a particular name, class, and type.
        /// </summary>
        RRSIG = 46,
        /// <summary>
        /// Next secure owener.
        /// </summary>
        NSEC = 47,
        /// <summary>
        /// Public key cryptography to sign and authenticate resource records.
        /// </summary>
        DNSKEY = 48,
        /// <summary>
        /// Authenticated next secure owner.
        /// </summary>
        NSEC3 = 50,
        /// <summary>
        /// Parameters needed by authoritative servers to calculate hashed owner names.
        /// </summary>
        NSEC3PARAM = 51,
        /// <summary>
        /// Shared secret key.
        /// </summary>
        TKEY = 249,
        /// <summary>
        /// Transactional Signature.
        /// </summary>
        TSIG = 250,
        /// <summary>
        /// A request for a transfer of an entire zone.
        /// </summary>
        AXFR = 252,
        /// <summary>
        /// A request for mailbox-related records (MB, MG or MR).
        /// </summary>
        MAILB = 253,
        /// <summary>
        /// A request for any record(s).
        /// </summary>
        ANY = 255,
        /// <summary>
        /// A Uniform Resource Identifier (URI) resource record.
        /// </summary>
        URI = 256,
        /// <summary>
        /// A certification authority authorization.
        /// </summary>
        CAA = 257
    }

    /// <summary>
    /// The values are maintained by IANA at https://www.iana.org/assignments/dns-parameters/dns-parameters.xhtml#dns-parameters-2.
    /// </summary>
    public enum DnsClass : ushort
    {
        /// <summary>
        /// The Internet.
        /// </summary>
        IN = 1,
        /// <summary>
        /// The CSNET class (Obsolete - used only for examples insome obsolete RFCs).
        /// </summary>
        CS = 2,
        /// <summary>
        /// The CHAOS class.
        /// </summary>
        CH = 3,
        /// <summary>
        /// Hesiod[Dyer 87].
        /// </summary>
        HS = 4,
        /// <summary>
        /// Used in UPDATE message to signify no class.
        /// </summary>
        None = 254,
        /// <summary>
        ///  Only used in QCLASS.
        /// </summary>
        ANY = 255
    }

    public struct DnsHead{

    }
    
    /// <summary>
    /// MDns Head  
    /// | TransactionID (2 bytes) | OpCode (2 bytes) | Dns Sign (2 byte)
    /// </summary>
    public struct MDnsHead {
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public ushort TransactionID { set; get; }


        /// <summary>
        /// The requested operation.
        /// </summary>
        /// <value></value>
        public DnsOpCode OpCode { set; get; }
        #region Sign Code
        /// <summary>
        /// A one bit field that specifies whether this message is a query(0), or a response(1).
        /// </summary>
        /// <value></value>
        public bool QR { set; get; }

        public bool AA { set; get; }

        public bool TC { set; get; }
        
        public bool RD { set; get; }
        public bool RA { set; get; }

        public byte OpCode4Bit { set; get; }
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        /// <value>Must be zero in all queries and responses.</value>
        public byte Z { set; get; }
        /// <summary>
        /// Authentic data.
        /// </summary>
        /// <value> true if the response data is authentic; otherwise, false.</value>
        public bool AD { get; set; }
        /// <summary>
        /// Checking disabled.
        /// </summary>
        /// <value>true if the query does not require authenticated data; otherwise, false.</value>
        public bool CD { get; set; }
        #endregion
    }

    public struct DnsQuestionRequest : IRequest
    {
        
        public byte ReplyCode { set; get; }
        public ushort QuestionsCount { set; get; }
        public ushort AnswerRRs { set; get; }
        public ushort AuthorityRRs { set; get; }
        public ushort Additional { set; get; }
        
        public List<byte[]> Data { set; get; }

        public byte QuestionType { set; get; }

        public byte QuestionClass { set; get; }


        public byte[] ToProtocolByteData()
        {
            throw new NotImplementedException();
        }

        public string ToProtocolData()
        {
            throw new NotImplementedException();
        }

    }
}

