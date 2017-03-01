using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace K2WorkflowUnitTest
{
    [TestClass]
    public class UnitTest1
    {

        private string folio;
        private string ProcessName;
        private int proceessInstance;
        private string SerialNumber;
        private string TaskAction;
        private int TaskCount = 0;
        private string Activities;
        private string DestinationUser;
        private string TaskActivity;
        private int milliseconds;
        private SourceCode.Workflow.Client.Actions Actions;
        private K2WorkflowMap.WorkflowInstanceFramework workflow;
        private Results Results;

        public int ProcInstId { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            this.folio = "I am a folio 3";
            this.ProcessName = @"K2Project2\TestProcess";
            this.TaskActivity = "Task";
            this.TaskAction = "Approve";
            this.Activities = "Setup;Task;Approve;End";
            this.TaskCount = 1;
            this.milliseconds = 3000;
            workflow = new K2WorkflowMap.WorkflowInstanceFramework();
            Results = new Results();
            Results.SetUpXML(this.ProcessName, this.folio);
        }


        /// <summary>
        /// Tests the starting of a workflow, should return a number greater than 0
        /// </summary>
        /// <param name="folio"></param>
        /// <param name="ProceesName"></param>
        /// <returns></returns>
        [TestMethod]
        public void StartWorkflow_Success()
        {

            int actual = workflow.StartProcess(this.folio, this.ProcessName);
            this.proceessInstance = actual;
            Results.SaveResult(TestType.ProcessInstance, this.proceessInstance.ToString());
            Assert.AreNotEqual(0, actual);
            ProcInstId = this.proceessInstance;

        }

        /// <summary>
        /// Test to check that the workflow has started, by checking it's status
        /// </summary>
        /// <param name="ProcessInstanceId"></param>
        [TestMethod]
        public void StartWorkflow_Running_Success()
        {

            int.TryParse(Results.GetResult(TestType.ProcessInstance), out this.proceessInstance);
            string actual = workflow.GetWorkflowStatus(this.proceessInstance);
            Results.SaveResult(TestType.StartStatus, actual);
            StringAssert.Equals(SourceCode.Workflow.Client.ProcessInstance.Status.Active.ToString(), actual);
        }



        /// <summary>
        /// Tests to see if tasks have been generated for this instance
        /// </summary>
        [TestMethod]
        public void WorkList_TasksFound_Success()
        {
            Thread.Sleep(milliseconds);
            int.TryParse(Results.GetResult(TestType.ProcessInstance), out this.proceessInstance);
            Boolean actual = false;
            actual = workflow.IsTaskFound(this.proceessInstance, this.ProcessName);
            Assert.AreEqual(true, actual);
        }


        /// <summary>
        /// Checks that the correct number of tasks is generated
        /// </summary>
        [TestMethod]
        public void WorkList_CorrectNumberOfTasksCreated_Success()
        {
            int.TryParse(Results.GetResult(TestType.ProcessInstance), out this.proceessInstance);
            int actual = 0;
            actual = workflow.GetTaskCount(this.proceessInstance, this.ProcessName);
            Results.SaveResult(TestType.TaskCount, actual.ToString());
            Assert.AreEqual(this.TaskCount, actual);
        }


        /// <summary>
        /// Gets the activity name of the task
        /// </summary>
        [TestMethod]
        public void WorkList_RetrieveTaskList_Success()
        {
            string Actual = string.Empty;
            int.TryParse(Results.GetResult(TestType.ProcessInstance), out this.proceessInstance);

            var task = workflow.GetTask(this.proceessInstance, this.ProcessName);
           
            this.SerialNumber = task[0].SerialNumber;
            Results.SaveResult(TestType.SerialNumber, this.SerialNumber);
            this.DestinationUser = task[0].Destination;
            Results.SaveResult(TestType.Destination, this.DestinationUser);
            Actual = task[0].ActivityName;
            Results.SaveResult(TestType.TaskActivity, Actual);


            Assert.AreEqual(this.TaskActivity, Actual);

        }
        /// <summary>
        /// Actions a task
        /// </summary>
        [TestMethod]
        public void Task_GetActions_Sucess()
        {
            Boolean Actual = false;
            this.SerialNumber = Results.GetResult(TestType.SerialNumber);
            this.DestinationUser = Results.GetResult(TestType.Destination);

            K2WorkflowMap.TaskDetails task = workflow.OpenTask(this.SerialNumber, this.DestinationUser)[0];
           Actual = workflow.ActionTask(this.TaskAction, this.SerialNumber, this.DestinationUser);
            Assert.AreEqual(true, Actual);

        }

        /// <summary>
        /// checks to see if task is complete by checking that task has gone
        /// </summary>
        [TestMethod]
        public void Task_CheckTaskComplete_Success()
        {
            int.TryParse(Results.GetResult(TestType.ProcessInstance), out this.proceessInstance);
            int actual = 0;
            actual = workflow.GetTaskCount(this.proceessInstance, this.ProcessName);
            Results.SaveResult(TestType.TaskCount, actual.ToString());
            Assert.AreEqual(0, actual);
        }


        /// <summary>
        /// Checks the correct number of activities ran
        /// </summary>
        [TestMethod]
        public void Activities_CompareNumberOfActivities_Success()
        {
           
            Thread.Sleep(milliseconds);
            int.TryParse(Results.GetResult(TestType.ProcessInstance), out this.proceessInstance);

            int NumberOfActivitiesRan = workflow.GetActivities(this.proceessInstance.ToString()).Count;
            String[] activities = this.Activities.Split(';');
            int count = 0;
            foreach (var activity in activities)
            {
                count++;

            }

            Assert.AreEqual(count, NumberOfActivitiesRan);
        }

        /// <summary>
        /// Checks the correct activities were executed in correct order
        /// </summary>
        /// 
        [TestMethod]
        public void Activities_CorrectActivitiesRan_Success()
        {
            Thread.Sleep(milliseconds);
            int.TryParse(Results.GetResult(TestType.ProcessInstance), out this.proceessInstance);

            Boolean actual = workflow.CompareActivities(this.proceessInstance, this.Activities);
            Assert.IsTrue(actual);
        }





    }
}
  