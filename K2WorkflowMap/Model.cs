using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2WorkflowMap
{

    public enum TestToRun
    {
        Start = 1,
        CheckStatus = 2,
        TaskExists = 3,
        TaskCount = 4,
        CheckTask = 5,
        OpenTaskDetails = 6,
        TaskActions = 7,
        CompleteTask = 8,
        GetActivities = 9

    }


    public class tasklist
    {
        public string ActInstDestID { get; set; }
        public string ActivityName { get; set; }
        public string EventName { get; set; }
        public string Destination { get; set; }
        public string Status { get; set; }
        public string SerialNumber { get; set; }

    }

    public class ActionList
    {
        public string ActionValue { get; set; }
        public string ActionText { get; set; }
    }

    public class TaskDetails
    {

        public string Status { get; set; }
        public string SerialNumber { get; set; }
        public SourceCode.Workflow.Client.Actions Actions { get; set; }
        public string Data { get; set; }
        public string DataFields { get; set; }
    }


    public class dataField
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string valueType { get; set; }
    }

    public class Activities
    {
        public int ActivityInstanceId { get; set; }
        public int ProcessInstanceId { get; set; }
        public string ActivityName { get; set; }


        public DateTime StartDate { get; set; }

        public DateTime FinishDate { get; set; }
        public String Status { get; set; }
    }

    public class Events
    {

        public int ActivityInstanceId { get; set; }
        public int ProcessInstanceId { get; set; }
        public string EventName { get; set; }
        public string Destination { get; set; }


        public DateTime StartDate { get; set; }

        public DateTime FinishDate { get; set; }
        public String Status { get; set; }
    }

    public class ActivityDesign
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class Workflow
    {
        public int ID { get; set; }
        public string Name { get; set; }

    }
}
