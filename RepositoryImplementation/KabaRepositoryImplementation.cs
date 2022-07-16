using KabaLockIntegration.Models;
using KabaLockIntegration.Repository;
using KabaLockIntegration.Utility;
using RestSharp;

namespace KabaLockIntegration.RepositoryImplementation
{
    public class KabaRepositoryImplementation : IKabaRepository
    {
        private readonly KabaDBContext _kabaDBContext;
        private readonly IConfiguration _configuration;

        public KabaRepositoryImplementation(KabaDBContext kabaDBContext, IConfiguration configuration)
        {
            _kabaDBContext = kabaDBContext;
            _configuration = configuration;
        }

        public BaseResponse<OTC> GenerateOTC(KabaRequestModel kabaRequest)
        {
            BaseResponse<OTC> baseResponse = new BaseResponse<OTC>();

            //var connStr = _configuration.GetSection("ConnectionStrings:PNBAlgoDB").Value;
            string content = string.Empty;
            string guiURL = string.Empty;

            var guiResult = FormGUIRequest(kabaRequest);
           
            if(guiResult != null)
            {
               content = guiResult.Item1;
               guiURL = guiResult.Item2;
            }
            else
            {
                baseResponse.Success = false;
                baseResponse.Message = Messages.Error_Message_GUI; ;
                return baseResponse;

            }

            if (!string.IsNullOrEmpty(content) && !string.IsNullOrEmpty(guiURL))
            {
                //save GUI response into database APILog
                Logging logging = new Logging();
                
                logging.DbLog("GUIRequest", guiURL, null, content, _kabaDBContext);

                //Map GUI data from string response into GUI Model
                GUIResponseModel gUIResponse =  GetGUIData(content);
                if (gUIResponse != null)
                {
                   if(!string.IsNullOrEmpty(gUIResponse.Status) && gUIResponse.Status == "1" && (gUIResponse.UserState == "0" || gUIResponse.UserState == "1" || gUIResponse.UserState == "2"))
                   {
                        string content1 = string.Empty;
                        string gliURL = string.Empty;

                        var gliResult = FormGLIRequest(kabaRequest);

                        if (gliResult != null)
                        {
                            content1 = gliResult.Item1;
                            gliURL = gliResult.Item2;
                        }
                        else
                        {
                            baseResponse.Success = false;
                            baseResponse.Message = Messages.Error_Message_GLI ;
                            return baseResponse;

                        }

                        if (!string.IsNullOrEmpty(content1) && !string.IsNullOrEmpty(gliURL))
                        {
                            //save GLI response into database APILog
                            logging.DbLog("GLIRequest", gliURL, null, content1, _kabaDBContext);

                            //Map GLI data from string response into GLI Model
                            GLIResponseModel gLIResponse = GetGLIData(content1);
                            if (gLIResponse != null)
                            {
                                if (!string.IsNullOrEmpty(gLIResponse.Status) && gLIResponse.Status == "1" && 
                                    gLIResponse.Lockstate == "0" && gLIResponse.Lockgrpstate == "1")
                                {                                    
                                    string content2 = string.Empty;
                                    string cuaURL = string.Empty;

                                    var cuaResult = FormCUARequest(kabaRequest);

                                    if (cuaResult != null)
                                    {
                                        content2 = cuaResult.Item1;
                                        cuaURL = cuaResult.Item2;
                                    }
                                    else
                                    {
                                        baseResponse.Success = false;
                                        baseResponse.Message = Messages.Error_Message_CUA ;
                                        return baseResponse;

                                    }

                                    if (!string.IsNullOrEmpty(content2) && !string.IsNullOrEmpty(cuaURL))
                                    {
                                        //save CUA response into database APILog
                                        logging.DbLog("CUARequest", cuaURL, null, content2, _kabaDBContext);

                                        //Map CUA data from string response into GUI Model
                                        CUAResponseModel cUAResponse = GetCUAData(content2);
                                        if (cUAResponse != null)
                                        {
                                            if (!string.IsNullOrEmpty(cUAResponse.Status) && cUAResponse.Status == "1")
                                            {
                                                if (cUAResponse.Assign == "1")
                                                {                                                    
                                                    string content3 = string.Empty;
                                                    string gOCURL = string.Empty;

                                                    var gOCResult = FormGOCRequest(kabaRequest);

                                                    if (gOCResult != null)
                                                    {
                                                        content3 = gOCResult.Item1;
                                                        gOCURL = gOCResult.Item2;
                                                    }
                                                    else
                                                    {
                                                        baseResponse.Success = false;
                                                        baseResponse.Message = Messages.Error_Message_GOC;
                                                        return baseResponse;

                                                    }
                                                    if (!string.IsNullOrEmpty(content3) && !string.IsNullOrEmpty(gOCURL))
                                                    {
                                                        //save GOC response into database APILog
                                                        logging.DbLog("GOCRequest", gOCURL, null, content3, _kabaDBContext);

                                                        //Map GOC data from string response into GUI Model
                                                        GOCResponseModel gOCResponse = GetGOCData(content3);
                                                        if (gOCResponse != null)
                                                        {
                                                            if (!string.IsNullOrEmpty(gOCResponse.Status) && gOCResponse.Status == "1")
                                                            {
                                                                return new BaseResponse<OTC>()
                                                                {
                                                                    Entity = new OTC()
                                                                    { Status = "1", OTC_PIN = gOCResponse.OpeningCode },
                                                                    Success = true,
                                                                    Message = "OTC Generated Successfully"
                                                                };
                                                            }
                                                            else
                                                            {
                                                                baseResponse.Success = false;
                                                                string response = ReturnErrorResponse(gOCResponse);
                                                                baseResponse.Message = response;
                                                                return baseResponse;
                                                            }
                                                        }
                                                    }
                                                   
                                                }
                                                else
                                                {
                                                    baseResponse.Success = false;
                                                    baseResponse.Message = Messages.UserNotAssignedToLock;
                                                    return baseResponse;
                                                }
                                            }
                                            else
                                            {
                                                baseResponse.Success = false;
                                                string response = ReturnErrorResponse(cUAResponse);
                                                baseResponse.Message = response;
                                                return baseResponse;
                                            }

                                        }
                                    }
                                    else
                                    {
                                        baseResponse.Success = false;                                       
                                        baseResponse.Message = Messages.Error_Message_CUA;
                                        return baseResponse;
                                    }
                                }
                                else
                                {
                                    baseResponse.Success = false;
                                    string response = string.Empty;
                                    if (gLIResponse.Lockstate == "1")
                                    {
                                        response = Messages.LockAlreadyOpen;
                                    }
                                    else if(gLIResponse.Lockgrpstate == "0")
                                    {
                                        response = Messages.LockGroupStateDisable;
                                    }
                                    else
                                    {
                                        response = ReturnErrorResponse(gLIResponse);
                                    }
                                    baseResponse.Message = response;
                                    return baseResponse;
                                }

                            }
                        }
                        else
                        {
                            baseResponse.Success = false;                           
                            baseResponse.Message = Messages.Error_Message_GLI;
                            return baseResponse;
                        }                      

                   }
                    else
                    {
                        baseResponse.Success = false;
                        string response = ReturnErrorResponse(gUIResponse);
                        baseResponse.Message = response;
                        return baseResponse;
                    }

                }
            }
            else
            {
                baseResponse.Success = false;
                baseResponse.Message = Messages.Error_Message_GUI;
                return baseResponse;
            }

            return baseResponse;
        }

                
        private GUIResponseModel GetGUIData(string content)
        {
            GUIResponseModel gUIResponse = new GUIResponseModel();

            var responseList = content.Split('&').ToList();
            if (responseList != null && responseList.Count > 0)
            {
                //Extract Models properties from response string from GUI
                foreach (var item in responseList)
                {
                    int index = item.IndexOf("=");
                    if (index >= 0)
                    {
                        if (item.Substring(0, index) == "command")
                        {
                            gUIResponse.Command = "GUI";
                            continue;
                        }

                        if (item.Substring(0, index) == "status")
                        {
                            var stat = item.Substring(index + 1);
                            gUIResponse.Status = stat;
                            continue;
                        }

                        if (item.Substring(0, index) == "userid")
                        {
                            var id = item.Substring(index + 1);
                            gUIResponse.UserId = id;
                            continue;
                        }

                        if (item.Substring(0, index) == "username")
                        {
                            var name = item.Substring(index + 1);
                            name = name.Replace("+", " ");
                            gUIResponse.UserName = name;

                            continue;
                        }

                        if (item.Substring(0, index) == "usercode")
                        {
                            var code = item.Substring(index + 1);
                            gUIResponse.UserCode = code;
                            continue;
                        }

                        if (item.Substring(0, index) == "userdescription")
                        {
                            var desc = item.Substring(index + 1);
                            desc = desc.Replace("+", " ");
                            gUIResponse.UserDescription = desc;
                            continue;
                        }

                        if (item.Substring(0, index) == "userinformation")
                        {
                            var info = item.Substring(index + 1);
                            gUIResponse.UserInformation = info;
                            continue;
                        }

                        if (item.Substring(0, index) == "userstate")
                        {
                            var state = item.Substring(index + 1);
                            gUIResponse.UserState = state;
                            continue;
                        }
                        //Error Cases
                        if (item.Substring(0, index) == "err")
                        {
                            var desc = item.Substring(index + 1);
                            gUIResponse.ErrorCode = desc;
                            continue;
                        }

                        if (item.Substring(0, index) == "parametername")
                        {
                            var info = item.Substring(index + 1);
                            gUIResponse.ErrorParameterName = info;
                            continue;
                        }

                        if (item.Substring(0, index) == "message")
                        {
                            var state = item.Substring(index + 1);
                            gUIResponse.ErrorMessage = state;
                            continue;
                        }

                        if (item.Substring(0, index) == "parametervalue")
                        {
                            var desc = item.Substring(index + 1);
                            desc = desc.Replace("+", " ");
                            gUIResponse.ErrorParameterValue = desc;
                            continue;
                        }

                        if (item.Substring(0, index) == "reason")
                        {
                            var info = item.Substring(index + 1);
                            gUIResponse.ErrorReason = info;
                            continue;
                        }

                    }
                }
            }

            return gUIResponse;
        }

        private GLIResponseModel GetGLIData(string content1)
        {
            GLIResponseModel cUAResponse = new GLIResponseModel();
            var responseList = content1.Split('&').ToList();
            if (responseList != null && responseList.Count > 0)
            {
                foreach (var item in responseList)
                {
                    int index = item.IndexOf("=");
                    if (index >= 0)
                    {
                        if (item.Substring(0, index) == "command")
                        {
                            cUAResponse.Command = "GLI";
                            continue;
                        }
                        if (item.Substring(0, index) == "status")
                        {
                            var stat = item.Substring(index + 1);
                            cUAResponse.Status = stat;
                            continue;
                        }
                        if (item.Substring(0, index) == "lockcode")
                        {
                            var stat = item.Substring(index + 1);
                            cUAResponse.Lockcode = stat;
                            continue;
                        }

                        if (item.Substring(0, index) == "lockdescription1")
                        {
                            var stat = item.Substring(index + 1);
                            cUAResponse.Lockdescription1 = stat;
                            continue;
                        }
                        if (item.Substring(0, index) == "lockdescription2")
                        {
                            var stat = item.Substring(index + 1);
                            cUAResponse.Lockdescription2 = stat;
                            continue;
                        }
                        if (item.Substring(0, index) == "locklocation")
                        {
                            var stat = item.Substring(index + 1);
                            cUAResponse.Locklocation = stat;
                            continue;
                        }

                        if (item.Substring(0, index) == "lockmode")
                        {
                            var stat = item.Substring(index + 1);
                            cUAResponse.Lockmode = stat;
                            continue;
                        }
                        if (item.Substring(0, index) == "lockstate")
                        {
                            var stat = item.Substring(index + 1);
                            cUAResponse.Lockstate = stat;
                            continue;
                        }
                        if (item.Substring(0, index) == "lockgrpname")
                        {
                            var stat = item.Substring(index + 1);
                            cUAResponse.Lockgrpname = stat;
                            continue;
                        }
                        if (item.Substring(0, index) == "lockgrpstate")
                        {
                            var stat = item.Substring(index + 1);
                            cUAResponse.Lockgrpstate = stat;
                            continue;
                        }
                        //Error Cases
                        if (item.Substring(0, index) == "err")
                        {
                            var desc = item.Substring(index + 1);
                            cUAResponse.ErrorCode = desc;
                            continue;
                        }
                   
                        if (item.Substring(0, index) == "parametername")
                        {
                            var info = item.Substring(index + 1);
                            cUAResponse.ErrorParameterName = info;
                            continue;
                        }

                        if (item.Substring(0, index) == "message")
                        {
                            var state = item.Substring(index + 1);
                            cUAResponse.ErrorMessage = state;
                            continue;
                        }

                        if (item.Substring(0, index) == "parametervalue")
                        {
                            var desc = item.Substring(index + 1);
                            desc = desc.Replace("+", " ");
                            cUAResponse.ErrorParameterValue = desc;
                            continue;
                        }

                        if (item.Substring(0, index) == "reason")
                        {
                            var info = item.Substring(index + 1);
                            cUAResponse.ErrorReason = info;
                            continue;
                        }
                    }
                }
            }
            return cUAResponse;
        }

        private CUAResponseModel GetCUAData(string content)
        {
            CUAResponseModel cUAResponse = new CUAResponseModel();
            var responseList = content.Split('&').ToList();
            if (responseList != null && responseList.Count > 0)
            {
                foreach (var item in responseList)
                {
                    int index = item.IndexOf("=");
                    if (index >= 0)
                    {
                        if (item.Substring(0, index) == "command")
                        {
                            cUAResponse.Command = "CUA";
                            continue;
                        }
                        if (item.Substring(0, index) == "status")
                        {
                            var stat = item.Substring(index + 1);
                            cUAResponse.Status = stat;
                            continue;
                        }
                        if (item.Substring(0, index) == "assign")
                        {
                            var stat = item.Substring(index + 1);
                            cUAResponse.Assign = stat;
                            continue;
                        }
                        //Error Cases
                        if (item.Substring(0, index) == "err")
                        {
                            var desc = item.Substring(index + 1);
                            cUAResponse.ErrorCode = desc;
                            continue;
                        }

                        if (item.Substring(0, index) == "parametername")
                        {
                            var info = item.Substring(index + 1);
                            cUAResponse.ErrorParameterName = info;
                            continue;
                        }

                        if (item.Substring(0, index) == "message")
                        {
                            var state = item.Substring(index + 1);
                            cUAResponse.ErrorMessage = state;
                            continue;
                        }

                        if (item.Substring(0, index) == "parametervalue")
                        {
                            var desc = item.Substring(index + 1);
                            desc = desc.Replace("+", " ");
                            cUAResponse.ErrorParameterValue = desc;
                            continue;
                        }

                        if (item.Substring(0, index) == "reason")
                        {
                            var info = item.Substring(index + 1);
                            cUAResponse.ErrorReason = info;
                            continue;
                        }
                    }
                }
            }
            return cUAResponse;
        }

        private GOCResponseModel GetGOCData(string content)
        {
            GOCResponseModel gOCResponse = new GOCResponseModel();
            var responseList = content.Split('&').ToList();
            if (responseList != null && responseList.Count > 0)
            {
                //Extract Models properties from response string from GUI
                foreach (var item in responseList)
                {
                    int index = item.IndexOf("=");
                    if (index >= 0)
                    {
                        if (item.Substring(0, index) == "command")
                        {
                            gOCResponse.Command = "GOC";
                            continue;
                        }

                        if (item.Substring(0, index) == "status")
                        {
                            var stat = item.Substring(index + 1);
                            gOCResponse.Status = stat;
                            continue;
                        }

                        if (item.Substring(0, index) == "openingcode")
                        {
                            var id = item.Substring(index + 1);
                            gOCResponse.OpeningCode = id;
                            continue;
                        }

                        //Error Cases
                        if (item.Substring(0, index) == "err")
                        {
                            var desc = item.Substring(index + 1);
                            gOCResponse.ErrorCode = desc;
                            continue;
                        }

                        if (item.Substring(0, index) == "parametername")
                        {
                            var info = item.Substring(index + 1);
                            gOCResponse.ErrorParameterName = info;
                            continue;
                        }

                        if (item.Substring(0, index) == "message")
                        {
                            var state = item.Substring(index + 1);
                            gOCResponse.ErrorMessage = state;
                            continue;
                        }

                        if (item.Substring(0, index) == "parametervalue")
                        {
                            var desc = item.Substring(index + 1);
                            desc = desc.Replace("+", " ");
                            gOCResponse.ErrorParameterValue = desc;
                            continue;
                        }

                        if (item.Substring(0, index) == "reason")
                        {
                            var info = item.Substring(index + 1);
                            gOCResponse.ErrorReason = info;
                            continue;
                        }

                    }
                }
            }

            return gOCResponse;
        }

        private Tuple<string,string> FormGUIRequest(KabaRequestModel kabaRequest)
        {
            string baseURL = System.Configuration.ConfigurationManager.AppSettings["BaseURL"];
            var _client = new RestClient();
            var gUIURL = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}", baseURL,
                System.Configuration.ConfigurationManager.AppSettings["Vocal"],
                System.Configuration.ConfigurationManager.AppSettings["Command"],
                System.Configuration.ConfigurationManager.AppSettings["CommandGUI"], "&",
                System.Configuration.ConfigurationManager.AppSettings["InPar1"],
                kabaRequest.OperatorId, "&",
                System.Configuration.ConfigurationManager.AppSettings["InPar2"], kabaRequest.UserCodeOrName);

            var restRequest = new RestRequest(gUIURL, Method.Get);
            var restReponse = _client.ExecuteAsync(restRequest);
            var content = restReponse.Result.Content;
            return new Tuple<string,string>(content, gUIURL);
        }

        private Tuple<string, string> FormGLIRequest(KabaRequestModel kabaRequest)
        {
            string baseURL = System.Configuration.ConfigurationManager.AppSettings["BaseURL"];
            var _client = new RestClient();
            var gLIURL = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}", baseURL,
            System.Configuration.ConfigurationManager.AppSettings["Vocal"],
            System.Configuration.ConfigurationManager.AppSettings["Command"],
            System.Configuration.ConfigurationManager.AppSettings["CommandGLI"], "&",
            System.Configuration.ConfigurationManager.AppSettings["InPar1"],
            kabaRequest.OperatorId, "&",
            System.Configuration.ConfigurationManager.AppSettings["InPar2"], kabaRequest.UserCodeOrName, "&",
            System.Configuration.ConfigurationManager.AppSettings["InPar3"] + kabaRequest.LockNameOrSerialNumber);

            var restRequest1 = new RestRequest(gLIURL, Method.Get);
            var restReponse1 = _client.ExecuteAsync(restRequest1);
            var content1 = restReponse1.Result.Content;
            return new Tuple<string, string>(content1, gLIURL);
        }

        private Tuple<string, string> FormCUARequest(KabaRequestModel kabaRequest)
        {
            string baseURL = System.Configuration.ConfigurationManager.AppSettings["BaseURL"];
            var _client = new RestClient();
            var cUAURL = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}", baseURL,
            System.Configuration.ConfigurationManager.AppSettings["Vocal"],
            System.Configuration.ConfigurationManager.AppSettings["Command"],
            System.Configuration.ConfigurationManager.AppSettings["CommandCUA"], "&",
            System.Configuration.ConfigurationManager.AppSettings["InPar1"],
            kabaRequest.OperatorId, "&",
            System.Configuration.ConfigurationManager.AppSettings["InPar2"], kabaRequest.UserCodeOrName, "&",
            System.Configuration.ConfigurationManager.AppSettings["InPar3"] + kabaRequest.LockNameOrSerialNumber);

            var restRequest1 = new RestRequest(cUAURL, Method.Get);
            var restReponse1 = _client.ExecuteAsync(restRequest1);
            var content1 = restReponse1.Result.Content;
            return new Tuple<string, string>(content1, cUAURL);
        }

        private Tuple<string, string> FormGOCRequest(KabaRequestModel kabaRequest)
        {
            string baseURL = System.Configuration.ConfigurationManager.AppSettings["BaseURL"];
            var _client = new RestClient();
            var gOCURL = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}", baseURL,
            System.Configuration.ConfigurationManager.AppSettings["Vocal"],
            System.Configuration.ConfigurationManager.AppSettings["Command"],
            System.Configuration.ConfigurationManager.AppSettings["CommandGOC"], "&",
            System.Configuration.ConfigurationManager.AppSettings["InPar1"],
            kabaRequest.OperatorId, "&",
            System.Configuration.ConfigurationManager.AppSettings["InPar2"], kabaRequest.UserCodeOrName, "&",
            System.Configuration.ConfigurationManager.AppSettings["InPar3"] + kabaRequest.LockNameOrSerialNumber, "&",
            System.Configuration.ConfigurationManager.AppSettings["InPar4"] + kabaRequest.OpeningMode);
            
            var restRequest1 = new RestRequest(gOCURL, Method.Get);
            var restReponse1 = _client.ExecuteAsync(restRequest1);
            var content1 = restReponse1.Result.Content;
            return new Tuple<string, string>(content1, gOCURL);
        }

        private string ReturnErrorResponse(GUIResponseModel response )
        {
            string message = string.Empty;

            if (string.IsNullOrEmpty(response.Status))
            {
                message = Messages.Error_Message;
            }
            else if(response.UserState == "3" || response.UserState == "4")
            {
                message = Messages.UserIsDisableorDelete;
            }
            else
            {
                if (response.Status == "0")
                {
                    if (response.ErrorCode == "00")
                    {
                        message = Messages.Err00_Message;
                        message = message +" " +response.ErrorMessage;
                    }
                    else if (response.ErrorCode == "01")
                    {
                        message = Messages.Err01_Message;
                        message = message + " " + Messages.UnknownMachine;
                    }
                    else if (response.ErrorCode == "02")
                    {
                        message = Messages.Err02_Message;
                        if (!string.IsNullOrEmpty(response.ErrorParameterName))
                            message = message +" Parameter Name - " +response.ErrorParameterName;
                    }
                    else if (response.ErrorCode == "03")
                    {
                        message = Messages.Err03_Message;
                        if (!string.IsNullOrEmpty(response.ErrorParameterName))
                            message = message + " Parameter Name - " + response.ErrorParameterName;
                        if (!string.IsNullOrEmpty(response.ErrorParameterValue))
                            message = message + ", Parameter Value - " + response.ErrorParameterValue; 
                    }
                    else
                    {
                        message = Messages.Err04_Message;
                        if (response.ErrorReason == "01")
                            message = message + Messages.Error_NotInterface;
                        else if (response.ErrorReason == "02")
                            message = message + Messages.Error_DisableInterface;
                        else
                            message = message + Messages.UserNotAssignedToOperator;
                    }
                }
            }
            return message;
        }

        private string ReturnErrorResponse(GLIResponseModel response)
        {
            string message = string.Empty;

            if (string.IsNullOrEmpty(response.Status))
            {
                message = Messages.Error_Message;
            }
            else
            {
                if (response.Status == "0")
                {
                    if (response.ErrorCode == "00")
                    {
                        message = Messages.Err00_Message;
                        message = message + " " + response.ErrorMessage;
                    }
                    else if (response.ErrorCode == "01")
                    {
                        message = Messages.Err01_Message;
                        message = message +" " +Messages.UnknownMachine;
                    }
                    else if (response.ErrorCode == "02")
                    {
                        message = Messages.Err02_Message;
                        if (!string.IsNullOrEmpty(response.ErrorParameterName))
                            message = message + " Parameter Name - " + response.ErrorParameterName;
                    }
                    else if (response.ErrorCode == "03")
                    {
                        message = Messages.Err03_Message;
                        if (!string.IsNullOrEmpty(response.ErrorParameterName))
                            message = message + " Parameter Name - " + response.ErrorParameterName;
                        if (!string.IsNullOrEmpty(response.ErrorParameterValue))
                            message = message + ",Parameter Value - " + response.ErrorParameterValue;
                    }
                    else
                    {
                        message = Messages.Err04_Message;
                        if (response.ErrorReason == "01")
                            message = message + Messages.Error_NotInterface;
                        else if (response.ErrorReason == "02")
                            message = message + Messages.Error_DisableInterface;
                        else
                            message = message + Messages.UserNotAssignedToOperator;
                    }
                }
            }
            return message;
        }

        private string ReturnErrorResponse(CUAResponseModel response)
        {
            string message = string.Empty;

            if (string.IsNullOrEmpty(response.Status))
            {
                message = Messages.Error_Message;
            }
            else
            {
                if (response.Status == "0")
                {
                    if (response.ErrorCode == "00")
                    {
                        message = Messages.Err00_Message;
                    }
                    else if (response.ErrorCode == "01")
                    {
                        message = Messages.Err01_Message;
                    }
                    else if (response.ErrorCode == "02")
                    {
                        message = Messages.Err02_Message;
                        if (!string.IsNullOrEmpty(response.ErrorParameterName))
                            message = message + " Parameter Name - " + response.ErrorParameterName;
                    }
                    else if (response.ErrorCode == "03")
                    {
                        message = Messages.Err03_Message;
                        if (!string.IsNullOrEmpty(response.ErrorParameterName))
                            message = message + " Parameter Name - " + response.ErrorParameterName;
                        if (!string.IsNullOrEmpty(response.ErrorParameterValue))
                            message = message + ",Parameter Value - " + response.ErrorParameterValue;

                    }
                    else if (response.ErrorCode == "04")
                    {
                        message = Messages.Err04_Message;
                        if (response.ErrorReason == "01")
                            message = message + Messages.Error_NotInterface;
                        else if (response.ErrorReason == "02")
                            message = message + Messages.Error_DisableInterface;
                        else if (response.ErrorReason == "03")
                            message = message + Messages.UserNotAssignedToOperator;
                    }
                }
            }
            return message;
        }

        private string ReturnErrorResponse(GOCResponseModel response)
        {
            string message = string.Empty;

            if (string.IsNullOrEmpty(response.Status))
            {
                message = Messages.Error_Message;
            }
            else
            {
                if (response.Status == "0")
                {
                    if (response.ErrorCode == "00")
                    {
                        message = Messages.Err00_Message;
                        message = message + " " + response.ErrorMessage;
                    }
                    else if (response.ErrorCode == "01")
                    {
                        message = Messages.Err01_Message;
                        message = message + " " + Messages.UnknownMachine;

                    }
                    else if (response.ErrorCode == "02")
                    {
                        message = Messages.Err02_Message;
                        if(!string.IsNullOrEmpty(response.ErrorParameterName))
                        message = message + " Parameter Name - " + response.ErrorParameterName;
                    }
                    else if (response.ErrorCode == "03")
                    {
                        message = Messages.Err03_Message;
                        if (!string.IsNullOrEmpty(response.ErrorParameterName))
                            message = message + " Parameter Name - " + response.ErrorParameterName;
                        if (!string.IsNullOrEmpty(response.ErrorParameterValue))
                            message = message + ",Parameter Value - " + response.ErrorParameterValue;

                    }
                    else if(response.ErrorCode =="04")
                    {
                        message = Messages.Err04_Message;
                        if (response.ErrorReason == "01")
                            message = message + Messages.Error_NotInterface;
                        else if (response.ErrorReason == "02")
                            message = message + Messages.Error_DisableInterface;
                        else if (response.ErrorReason == "03")
                            message = message + Messages.UserNotAssignedToOperator;
                        else if (response.ErrorReason == "04")
                            message = message + Messages.UserNotAssignedToLock;
                        else if (response.ErrorReason == "05")
                            message = message + Messages.Error_LockNotInRightState;
                        else if (response.ErrorReason == "06")
                            message = message + Messages.Error_RandomIdIsNotActivitated;                     
                        else
                            message = message + Messages.ErrorUser2IsMissing;
                    }
                    else
                    {
                        message = Messages.Error46_Message;
                    }
                }
            }
            return message;
        }

        private string ReturnErrorResponse(CDSResponseModel response)
        {
            string message = string.Empty;

            if (string.IsNullOrEmpty(response.Status))
            {
                message = Messages.Error_Message;
            }
            else
            {
                if (response.Status == "0")
                {
                    if (response.ErrorCode == "00")
                    {
                        message = Messages.Err00_Message;
                        message = message + " " + response.ErrorMessage;
                    }
                    else if (response.ErrorCode == "01")
                    {
                        message = Messages.Err01_Message;
                        message = message + " " + Messages.UnknownMachine;

                    }
                    else if (response.ErrorCode == "02")
                    {
                        message = Messages.Err02_Message;
                        if (!string.IsNullOrEmpty(response.ErrorParameterName))
                            message = message + " Parameter Name - " + response.ErrorParameterName;
                    }
                    else if (response.ErrorCode == "03")
                    {
                        message = Messages.Err03_Message;
                        if (!string.IsNullOrEmpty(response.ErrorParameterName))
                            message = message + " Parameter Name - " + response.ErrorParameterName;
                        if (!string.IsNullOrEmpty(response.ErrorParameterValue))
                            message = message + ",Parameter Value - " + response.ErrorParameterValue;

                    }
                    else if (response.ErrorCode == "04")
                    {
                        message = Messages.Err04_Message;
                        if (response.ErrorReason == "01")
                            message = message + Messages.Error_NotInterface;
                        else if (response.ErrorReason == "02")
                            message = message + Messages.Error_DisableInterface;
                        else if (response.ErrorReason == "03")
                            message = message + Messages.UserNotAssignedToOperator;
                        else if (response.ErrorReason == "04")
                            message = message + Messages.UserNotAssignedToLock;
                        else if (response.ErrorReason == "05")
                            message = message + Messages.Error_LockNotInRightState;
                        else if (response.ErrorReason == "06")
                            message = message + Messages.Error_RandomIdIsNotActivitated;
                        else
                            message = message + Messages.ErrorUser2IsMissing;
                    }
                    else
                    {
                        message = Messages.Error34_Message;
                    }
                }
            }
            return message;
        }


        public BaseResponse<CloseSealResponse> CloseSeal(DecodeCloseSealModel kabaRequest)
        {

            BaseResponse<CloseSealResponse> baseResponse = new BaseResponse<CloseSealResponse>();

            string content = string.Empty;
            string guiURL = string.Empty;
            var guiResult = FormGUIRequest(new KabaRequestModel() { LockNameOrSerialNumber = kabaRequest.LockNameOrSerialNumber ,OperatorId = kabaRequest.OperatorId ,UserCodeOrName = kabaRequest.UserCodeOrName});

            if (guiResult != null)
            {
                content = guiResult.Item1;
                guiURL = guiResult.Item2;
            }
            else
            {
                baseResponse.Success = false;
                baseResponse.Message = Messages.Error_Message_GUI; ;
                return baseResponse;

            }

            if (!string.IsNullOrEmpty(content) && !string.IsNullOrEmpty(guiURL))
            {
                //save GUI response into database APILog
                Logging logging = new Logging();

                logging.DbLog("GUIRequest", guiURL, null, content, _kabaDBContext);

                //Map GUI data from string response into GUI Model
                GUIResponseModel gUIResponse = GetGUIData(content);
                if (gUIResponse != null)
                {
                    if (!string.IsNullOrEmpty(gUIResponse.Status) && gUIResponse.Status == "1")
                    {
                        string content1 = string.Empty;
                        string gliURL = string.Empty;

                        var gliResult = FormGLIRequest(new KabaRequestModel() { LockNameOrSerialNumber = kabaRequest.LockNameOrSerialNumber, OperatorId = kabaRequest.OperatorId, UserCodeOrName = kabaRequest.UserCodeOrName });

                        if (gliResult != null)
                        {
                            content1 = gliResult.Item1;
                            gliURL = gliResult.Item2;
                        }
                        else
                        {
                            baseResponse.Success = false;
                            baseResponse.Message = Messages.Error_Message_GLI;
                            return baseResponse;

                        }

                        if (!string.IsNullOrEmpty(content1) && !string.IsNullOrEmpty(gliURL))
                        {
                            //save GLI response into database APILog
                            logging.DbLog("GLIRequest", gliURL, null, content1, _kabaDBContext);

                            //Map GLI data from string response into GLI Model
                            GLIResponseModel gLIResponse = GetGLIData(content1);
                            if (gLIResponse != null)
                            {
                                if (!string.IsNullOrEmpty(gLIResponse.Status) && gLIResponse.Status == "1" &&
                                    gLIResponse.Lockstate == "1" && gLIResponse.Lockgrpstate == "1")
                                {
                                    string content2 = string.Empty;
                                    string cuaURL = string.Empty;

                                    var cuaResult = FormCUARequest(new KabaRequestModel() { LockNameOrSerialNumber = kabaRequest.LockNameOrSerialNumber, OperatorId = kabaRequest.OperatorId, UserCodeOrName = kabaRequest.UserCodeOrName });

                                    if (cuaResult != null)
                                    {
                                        content2 = cuaResult.Item1;
                                        cuaURL = cuaResult.Item2;
                                    }
                                    else
                                    {
                                        baseResponse.Success = false;
                                        baseResponse.Message = Messages.Error_Message_CUA;
                                        return baseResponse;

                                    }

                                    if (!string.IsNullOrEmpty(content2) && !string.IsNullOrEmpty(cuaURL))
                                    {
                                        //save CUA response into database APILog
                                        logging.DbLog("CUARequest", cuaURL, null, content2, _kabaDBContext);

                                        //Map CUA data from string response into GUI Model
                                        CUAResponseModel cUAResponse = GetCUAData(content2);
                                        if (cUAResponse != null)
                                        {
                                            if (!string.IsNullOrEmpty(cUAResponse.Status) && cUAResponse.Status == "1")
                                            {
                                                if (cUAResponse.Assign == "1")
                                                {
                                                    string content3 = string.Empty;
                                                    string dCSURL = string.Empty;

                                                    var dCSResult = FormDCSRequest(kabaRequest);

                                                    if (dCSResult != null)
                                                    {
                                                        content = dCSResult.Item1;
                                                        dCSURL = dCSResult.Item2;
                                                    }
                                                    else
                                                    {
                                                        baseResponse.Success = false;
                                                        baseResponse.Message = Messages.Error_Message_DCS; ;
                                                        return baseResponse;

                                                    }

                                                    if (!string.IsNullOrEmpty(content) && !string.IsNullOrEmpty(dCSURL))
                                                    {
                                                        
                                                        //save CDS response into database APILog
                                                        logging.DbLog("CDSRequest", dCSURL, null, content, _kabaDBContext);

                                                        //Map CDS data from string response into CDS Model
                                                        CDSResponseModel cDSResponse = GetCDSData(content);
                                                        if (cDSResponse != null)
                                                        {
                                                            if (!string.IsNullOrEmpty(cDSResponse.Status) && cDSResponse.Status == "1"                                && cDSResponse.ConfLockClosed == "1")
                                                            {
                                                                return new BaseResponse<CloseSealResponse>()
                                                                {
                                                                 Entity = new CloseSealResponse() { Status = cDSResponse.Status, ConflockClosed = cDSResponse.ConfLockClosed,InfoStatus = cDSResponse.InfoStatus,InfoDuress = cDSResponse.InfoDuress,InfoBattery = cDSResponse.InfoBattery },
                                                                Success = true,
                                                                Message = "Code Seal Decoded Successfully"
                                                                };
                                                            }
                                                            else
                                                            {
                                                                baseResponse.Success = false;
                                                                string response = ReturnErrorResponse(cDSResponse);
                                                                baseResponse.Message = response;
                                                                return baseResponse;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        baseResponse.Success = false;
                                                        baseResponse.Message = Messages.Error_Message_DCS;
                                                        return baseResponse;
                                                    }

                                                }
                                                else
                                                {
                                                    baseResponse.Success = false;
                                                    baseResponse.Message = Messages.UserNotAssignedToLock;
                                                    return baseResponse;
                                                }
                                            }
                                            else
                                            {
                                                baseResponse.Success = false;
                                                string response = ReturnErrorResponse(cUAResponse);
                                                baseResponse.Message = response;
                                                return baseResponse;
                                            }

                                        }
                                    }
                                    else
                                    {
                                        baseResponse.Success = false;
                                        baseResponse.Message = Messages.Error_Message_CUA;
                                        return baseResponse;
                                    }
                                }
                                else
                                {
                                    baseResponse.Success = false;
                                    string response = string.Empty;
                                    if (gLIResponse.Lockstate == "0")
                                    {
                                        response = Messages.LockAlreadyClosed;
                                    }
                                    else
                                    {
                                        response = ReturnErrorResponse(gLIResponse);
                                    }
                                    baseResponse.Message = response;
                                    return baseResponse;
                                }

                            }
                        }
                        else
                        {
                            baseResponse.Success = false;
                            baseResponse.Message = Messages.Error_Message_GLI;
                            return baseResponse;
                        }

                    }
                    else
                    {
                        baseResponse.Success = false;
                        string response = ReturnErrorResponse(gUIResponse);
                        baseResponse.Message = response;
                        return baseResponse;
                    }

                }
            }
            else
            {
                baseResponse.Success = false;
                baseResponse.Message = Messages.Error_Message_GUI;
                return baseResponse;
            }          

            return baseResponse;
        }

        private CDSResponseModel GetCDSData(string content)
        {
            CDSResponseModel response = new CDSResponseModel();
            var responseList = content.Split('&').ToList();
            if (responseList != null && responseList.Count > 0)
            {
                foreach (var item in responseList)
                {
                    int index = item.IndexOf("=");
                    if (index >= 0)
                    {
                        if (item.Substring(0, index) == "command")
                        {
                            response.Command = "CDS";
                            continue;
                        }
                        if (item.Substring(0, index) == "status")
                        {
                            var stat = item.Substring(index + 1);
                            response.Status = stat;
                            continue;
                        }
                        if (item.Substring(0, index) == "conflockclosed")
                        {
                            var stat = item.Substring(index + 1);
                            response.ConfLockClosed = stat;
                            continue;
                        }
                        if (item.Substring(0, index) == "infostatus")
                        {
                            var stat = item.Substring(index + 1);
                            response.InfoStatus = stat;
                            continue;
                        }
                        if (item.Substring(0, index) == "infoduress")
                        {
                            var stat = item.Substring(index + 1);
                            response.InfoDuress = stat;
                            continue;
                        }
                        if (item.Substring(0, index) == "infobattery")
                        {
                            var stat = item.Substring(index + 1);
                            response.InfoBattery = stat;
                            continue;
                        }
                        
                        //Error Cases
                        if (item.Substring(0, index) == "err")
                        {
                            var desc = item.Substring(index + 1);
                            response.ErrorCode = desc;
                            continue;
                        }

                        if (item.Substring(0, index) == "parametername")
                        {
                            var info = item.Substring(index + 1);
                            response.ErrorParameterName = info;
                            continue;
                        }

                        if (item.Substring(0, index) == "message")
                        {
                            var state = item.Substring(index + 1);
                            response.ErrorMessage = state;
                            continue;
                        }

                        if (item.Substring(0, index) == "parametervalue")
                        {
                            var desc = item.Substring(index + 1);
                            desc = desc.Replace("+", " ");
                            response.ErrorParameterValue = desc;
                            continue;
                        }

                        if (item.Substring(0, index) == "reason")
                        {
                            var info = item.Substring(index + 1);
                            response.ErrorReason = info;
                            continue;
                        }
                    }
                }
            }
            return response;
        }

        private Tuple<string, string> FormDCSRequest(DecodeCloseSealModel kabaRequest)
        {
            string baseURL = System.Configuration.ConfigurationManager.AppSettings["BaseURL"];
            var _client = new RestClient();
            var dCSURL = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}", baseURL,
            System.Configuration.ConfigurationManager.AppSettings["Vocal"],
            System.Configuration.ConfigurationManager.AppSettings["Command"],
            System.Configuration.ConfigurationManager.AppSettings["CommandDCS"], "&",
            System.Configuration.ConfigurationManager.AppSettings["InPar1"],
            kabaRequest.OperatorId, "&",
            System.Configuration.ConfigurationManager.AppSettings["InPar2"], kabaRequest.UserCodeOrName, "&",
            System.Configuration.ConfigurationManager.AppSettings["InPar3"] + kabaRequest.LockNameOrSerialNumber, "&",
            System.Configuration.ConfigurationManager.AppSettings["CloseSeal"] + "=" + kabaRequest.CloseSeal);

            var restRequest1 = new RestRequest(dCSURL, Method.Get);
            var restReponse1 = _client.ExecuteAsync(restRequest1);
            var content1 = restReponse1.Result.Content;
            return new Tuple<string, string>(content1, dCSURL);
        }
    }
    
}
