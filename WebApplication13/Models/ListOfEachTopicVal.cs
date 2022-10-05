using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication13.Models
{
    public class ListOfEachTopicVal
    {
        public string TopicName { get; set; }
        public JsonPropForTopAB[] AllPlanWithValInTopic { get; set; }
    }
}