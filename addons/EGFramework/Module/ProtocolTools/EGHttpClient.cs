using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EGFramework{
    public class EGHttpClient : IEGFramework, IModule
    {
        public HttpClient HTTPClient { set; get; } = new HttpClient();

        public Encoding StringEncoding { set; get; } = Encoding.UTF8;

        public void Init()
        {

        }

        public async Task<TResponse> HttpRequest<TResponse>(string hostUrl,IRequest request,ProtocolType protocolType = ProtocolType.HttpGet,EGFormData formData = null) where TResponse : IResponse,new(){
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                HttpContent httpContent = new StringContent("",Encoding.UTF8,"application/json");
                if(formData != null){
                    MultipartFormDataContent formContent = new MultipartFormDataContent();
                    foreach(KeyValuePair<string,string> pair in formData.FormStrings){
                        formContent.Add(new StringContent(pair.Value,Encoding.UTF8),pair.Key);
                    }
                    foreach(KeyValuePair<string,byte[]> pair in formData.FormBytes){
                        formContent.Add(new ByteArrayContent(pair.Value),pair.Key,pair.Key+"."+formData.Suffix);
                    }
                    foreach(KeyValuePair<string,Stream> pair in formData.FormStreams){
                        formContent.Add(new StreamContent(pair.Value),pair.Key,pair.Key+"."+formData.Suffix);
                    }
                    httpContent = formContent;
                }
                else if(request.ToProtocolData() != null && request.ToProtocolData() != ""){
                    httpContent = new StringContent(request.ToProtocolData(),Encoding.UTF8,"application/json");
                }else if (request.ToProtocolByteData() != null){
                    httpContent = new ByteArrayContent(request.ToProtocolByteData());
                }
                HttpResponseMessage httpResponse;
                switch(protocolType){
                    case ProtocolType.HttpGet:
                        httpResponse = await HTTPClient.GetAsync(hostUrl);
                        break;
                    case ProtocolType.HttpPost:
                        httpResponse = await HTTPClient.PostAsync(hostUrl,httpContent);
                        break;
                    case ProtocolType.HttpPut:
                        httpResponse = await HTTPClient.PutAsync(hostUrl,httpContent);
                        break;
                    case ProtocolType.HttpPatch:
                        httpResponse = await HTTPClient.PatchAsync(hostUrl,httpContent);
                        break;
                    case ProtocolType.HttpDelete:
                        httpResponse = await HTTPClient.DeleteAsync(hostUrl);
                        break;
                    default:
                        httpResponse = await HTTPClient.GetAsync(hostUrl);
                        break;
                }
                httpResponse.EnsureSuccessStatusCode();
                byte[] responseBytes = await httpResponse.Content.ReadAsByteArrayAsync();
                string responseBody = await httpResponse.Content.ReadAsStringAsync();
                TResponse response = new TResponse();
                response.TrySetData(responseBody,responseBytes);
                return response;
            }
            catch (HttpRequestException e)
            {
                Godot.GD.Print("Exception Message : "+e.Message);
                return default;
            }
        }

        public IArchitecture GetArchitecture()
        {
            return EGArchitectureImplement.Interface;
        }
    }

    public class EGFormData{
        public string Suffix = "";
        public Dictionary<string,string> FormStrings = new Dictionary<string, string>();
        public Dictionary<string,byte[]> FormBytes = new Dictionary<string, byte[]>();
        public Dictionary<string,System.IO.Stream> FormStreams = new Dictionary<string, System.IO.Stream>();
    }
    public static class CanGetEGHttpClientExtension{
        public static EGHttpClient EGHttpClient(this IEGFramework self){
            return self.GetModule<EGHttpClient>();
        }
    }
}