namespace SampleEndpoint
{
    using System;
    using System.Runtime.Serialization;
    using Newtonsoft.Json.Serialization;

    class SkipConstructorContractResolver : DefaultContractResolver
    {
        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            var jsonContract = base.CreateObjectContract(objectType);
            jsonContract.DefaultCreator = () => FormatterServices.GetUninitializedObject(objectType);
            return jsonContract;
        }

    }
}