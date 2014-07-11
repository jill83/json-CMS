using System;
using System.Runtime.Remoting.Messaging;
using Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonCMS.Tests.Builders
{
    internal class JsonFileBuilder
    {
        private int id;
        private string jsonString;
        private string fileName;
        private DateTime? deleted = null;
        private bool inUse;

        public JsonFileBuilder()
        {

        }


        public static implicit operator JsonFile(JsonFileBuilder builder)
        {
            return builder.Build();
        }

        public JsonFile Build()
        {
            return new JsonFile
            {
                Id = id,
                FileName = fileName,
                Deleted = deleted,
                JsonString = jsonString,
                InUse = inUse
            };
        }

        public JsonFileBuilder WithName(string fileName)
        {
            this.fileName = fileName;
            return this;
        }

    }
}
