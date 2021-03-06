﻿using Nancy;

namespace OmniSharp.FindTypes
{
    public class FindTypesModule : NancyModule
    {
        public FindTypesModule(FindTypesHandler handler)
        {
            Post["/findtypes"] = x =>
                {
                    var res = handler.FindAllTypes();
                    return Response.AsJson(res);
                };
        }
    }
}