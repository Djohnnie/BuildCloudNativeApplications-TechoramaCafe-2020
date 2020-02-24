using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpWars.Common.Configuration.Interfaces;
using CSharpWars.DtoModel;
using CSharpWars.Validator;
using CSharpWars.Web.Helpers.Interfaces;
using Grpc.Net.Client;
using ScriptValidationMessage = CSharpWars.DtoModel.ScriptValidationMessage;

namespace CSharpWars.Web.Helpers
{
    public class ScriptValidationHelper : IScriptValidationHelper
    {
        private readonly IConfigurationHelper _configurationHelper;

        public ScriptValidationHelper(
            IConfigurationHelper configurationHelper)
        {
            _configurationHelper = configurationHelper;
        }

        public async Task<ValidatedScriptDto> Validate(ScriptToValidateDto script)
        {
            try
            {
                var request = new ScriptValidationRequest { Script = script.Script };

                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                var channel = GrpcChannel.ForAddress(_configurationHelper.ValidationHost);
                var client = new ScriptValidator.ScriptValidatorClient(channel);
                var response = await client.ValidateAsync(request);

                return new ValidatedScriptDto
                {
                    Script = script.Script,
                    CompilationTimeInMilliseconds = response.CompilationTimeInMilliseconds,
                    RunTimeInMilliseconds = response.RunTimeInMilliseconds,
                    Messages = new List<ScriptValidationMessage>(response.ValidationMessages.Select(x => new ScriptValidationMessage
                    {
                        Message = x.Message,
                        LocationStart = x.LocationStart,
                        LocationEnd = x.LocationEnd
                    }))
                };
            }
            catch
            {
                return null;
            }
        }
    }
}