using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApiAiSDK;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace Adfa.Bot.Builder.Luis.ApiAi
{
    [Serializable]
    public class ApiAiLuisService : ILuisService
    {
        private readonly string _token;

        private string _text;

        public ApiAiLuisService(string witToken)
        {
            _token = witToken;
        }

        public Task<LuisResult> QueryAsync(Uri uri, CancellationToken token)
        {
            var client = new ApiAiSDK.ApiAi(new AIConfiguration(_token, SupportedLanguage.English));

            var response = client.TextRequest(_text);
            var message = response.IsError ? response.Result : null;

            return Task.FromResult(
                new LuisResult(
                    _text,
                    message?.Parameters
                        .Select(p => new EntityRecommendation(type: p.Key, entity: p.Value.ToString())).ToList() ?? Enumerable.Empty<EntityRecommendation>().ToList(),
                    topScoringIntent: !string.IsNullOrWhiteSpace(message?.Action) ? new IntentRecommendation(message.Action, message.Score) : null
                )
            );
        }

        public Uri BuildUri(LuisRequest luisRequest)
        {
            return BuildUri(luisRequest.Query);
        }

        private Uri BuildUri(string text)
        {
            _text = text;
            return new Uri("https://api.api.ai/v1/query?v=20150910&query=" + Uri.EscapeUriString(text));
        }
    }
}