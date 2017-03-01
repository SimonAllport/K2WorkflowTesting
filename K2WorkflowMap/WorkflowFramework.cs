using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace K2WorkflowMap
{



    public class WorkflowInstanceFramework : Iworkflowframeworkcs
    {


        public string servername = "localhost";
        public string ServiceAccount = @"denallix\administrator";





        /// <summary>
        /// Starts a workflow
        /// </summary>
        /// <param name="Folio"></param>
        /// <param name="ProcessName"></param>
        /// <returns></returns>
        public  int StartProcess(string Folio, string ProcessName)
        {
            var con = ConfigurationManager.AppSettings;


            int ProcessInstanceId = 0;
            SourceCode.Workflow.Client.Connection K2Conn = new SourceCode.Workflow.Client.Connection();
            K2Conn.Open(servername);
            try
            {
                SourceCode.Workflow.Client.ProcessInstance K2Proc = K2Conn.CreateProcessInstance(ProcessName);
              
                K2Proc.Folio = Folio;
                K2Conn.ImpersonateUser(ServiceAccount);
                K2Conn.StartProcessInstance(K2Proc);
                ProcessInstanceId = K2Proc.ID;
            }
            catch (Exception EX)
            {
                ProcessInstanceId = 0;
            }

            finally
            {
                K2Conn.Close();
            }
            return ProcessInstanceId;
        }

        /// <summary>
        /// Checks to see if there is an task
        /// </summary>
        /// <param name="ProcessInstanceId"></param>
        /// <param name="ProcessName"></param>
        /// <returns></returns>
        public  Boolean IsTaskFound(int ProcessInstanceId, string ProcessName)
        {
            Boolean Result = false;
            SourceCode.Workflow.Management.WorkflowManagementServer wrkmgt = new SourceCode.Workflow.Management.WorkflowManagementServer(servername, 5555);
            SourceCode.Workflow.Management.WorklistItems worklistItems = null;
            wrkmgt.Open();
            try
            {
                worklistItems = wrkmgt.GetWorklistItems("", "", "", "", "", "", "");


                foreach (SourceCode.Workflow.Management.WorklistItem worklistItem in worklistItems)
                {
                    if (worklistItem.ProcInstID == ProcessInstanceId)
                    {
                        Result = true;
                    }
                }
            }
            catch (Exception ex)
            {

                Result = false;

            }
            finally
            {
                wrkmgt.Connection.Close();
            }

            return Result;
        }



        /// <summary>
        /// Gets the number of active tasks for instance of a workflow
        /// </summary>
        /// <param name="ProcessInstanceId"></param>
        /// <returns></returns>
        public  int GetTaskCount(int ProcessInstanceId,string ProcessName)
        {
            int Result = 0;
            int count = 0;
            SourceCode.Workflow.Management.WorkflowManagementServer wrkmgt = new SourceCode.Workflow.Management.WorkflowManagementServer(servername ,5555);
            SourceCode.Workflow.Management.WorklistItems worklistItems = null;
            wrkmgt.Open();
            try
            {
                worklistItems = wrkmgt.GetWorklistItems("", "", "", "", "", "", "");

                foreach (SourceCode.Workflow.Management.WorklistItem worklistItem in worklistItems)
                {
                    if (worklistItem.ProcInstID == ProcessInstanceId)
                    {
                        count++;
                    }
                }
                Result = count;
            }

            catch (Exception ex)
            {

                Result = 0;

            }
            finally
            {
                wrkmgt.Connection.Close();
            }

            return Result;
        }

        /// <summary>
        /// Gets details about the task
        /// </summary>
        /// <param name="ProcessInstanceId"></param>
        /// <returns></returns>
        public  List<tasklist> GetTask(int ProcessInstanceId,string ProcessName)
        {

            List<tasklist> list = new List<tasklist>();
            SourceCode.Workflow.Management.WorkflowManagementServer wrkmgt = new SourceCode.Workflow.Management.WorkflowManagementServer(servername, 5555);
            SourceCode.Workflow.Management.WorklistItems worklistItems = null;
            wrkmgt.Open();
            try
            {
                worklistItems = wrkmgt.GetWorklistItems("", "", "", "", "", "", "");

                foreach (SourceCode.Workflow.Management.WorklistItem worklistItem in worklistItems)
                {
                    if (worklistItem.ProcInstID == ProcessInstanceId)
                    {
                        var x = worklistItem.ActivityName;

                        list.Add(new tasklist
                        {
                            Status = worklistItem.Status.ToString(),
                            Destination = worklistItem.Destination,
                            EventName = worklistItem.EventName,
                            ActInstDestID = worklistItem.ActInstDestID.ToString(),
                            ActivityName = worklistItem.ActivityName,
                            SerialNumber = (ProcessInstanceId + "_" + worklistItem.ActInstDestID)


                        });



                    }
                }
            }
            catch (Exception ex)
            {

                // Result = false;

            }
            finally
            {
                wrkmgt.Connection.Close();
            }

            return list;
        }


        /// <summary>
        /// Task Details
        /// </summary>
        /// <param name="serialnumber"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public List<TaskDetails> OpenTask(string serialnumber, string destination)
        {

            List<TaskDetails> list = new List<TaskDetails>();

            SourceCode.Workflow.Client.Connection K2Conn = new SourceCode.Workflow.Client.Connection();
            K2Conn.Open(servername);
            K2Conn.ImpersonateUser(destination);
            SourceCode.Workflow.Client.WorklistItem K2WListItem = K2Conn.OpenWorklistItem(serialnumber);

            try
            {

                list.Add(new TaskDetails
                {
                    Status = K2WListItem.Status.ToString(),
                    SerialNumber = K2WListItem.SerialNumber,
                    Actions = K2WListItem.Actions,
                    Data = K2WListItem.Data,
                    DataFields = K2WListItem.ProcessInstance.DataFields.ToString()
                });


                K2WListItem.Release();
            }
            catch (Exception ex)
            {
                list.Add(new TaskDetails
                {
                    Status = "No Task"
                });

            }
            finally {
                K2Conn.Close();
            }
            return list;
        }

        /// <summary>
        /// List of Actions
        /// </summary>
        /// <param name="actions"></param>
        /// <returns></returns>
        public  List<ActionList> WhatCanIdo(SourceCode.Workflow.Client.Actions actions)
        {
            List<ActionList> list = new List<ActionList>();
            if (actions != null)
            {
                foreach (SourceCode.Workflow.Client.Action action in actions)
                {
                    list.Add(new ActionList
                    {
                        ActionText = action.Name,
                        ActionValue = action.Name
                    });


                }
            }
            else {

                list.Add(new ActionList
                {
                    ActionText = "No Action",
                    ActionValue = "No Action"
                });
            }

            return list;
        }




        /// <summary>
        /// Actions a task
        /// </summary>
        /// <param name="action"></param>
        /// <param name="serialnumber"></param>
        /// <param name="destinationuser"></param>
        /// <returns></returns>
        public  Boolean ActionTask(string action, string serialnumber, string destinationuser)
        {
            Boolean result = false;
            SourceCode.Workflow.Client.Connection K2Conn = new SourceCode.Workflow.Client.Connection();
            K2Conn.Open(servername);
            K2Conn.ImpersonateUser(destinationuser);
            SourceCode.Workflow.Client.WorklistItem K2WListItem = K2Conn.OpenWorklistItem(serialnumber);
            try
            {

                K2WListItem.Actions[action].Execute();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            finally {
                K2Conn.Close();
            }
            return result;
        }

        /// <summary>
        /// Checks to see if the task count has decreased
        /// </summary>
        /// <param name="ProcInst"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        public  Boolean IsTaskComplete(int ProcInst, int Count,string processname)
        {
            Boolean result = false;
            int LastCount = Count;
            int NewCount = GetTaskCount(ProcInst,processname);
            if (LastCount == Count)
            {
                result = false;
            }
            else
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Checks to see if the actual task that was actioned has been successful
        /// </summary>
        /// <param name="action"></param>
        /// <param name="serialnumber"></param>
        /// <param name="destinationuser"></param>
        /// <returns></returns>
        public  Boolean IsTaskComplete(string action, string serialnumber, string destinationuser)
        {
            Boolean result = true;
            SourceCode.Workflow.Client.Connection K2Conn = new SourceCode.Workflow.Client.Connection();
            K2Conn.Open(servername);
            K2Conn.ImpersonateUser(destinationuser);
            SourceCode.Workflow.Client.WorklistItem K2WListItem = K2Conn.OpenWorklistItem(serialnumber);
            try
            {

                K2WListItem.Actions[action].Execute();
                result = false;
            }
            catch (Exception ex)
            {
                result = true;
            }
            finally
            {
                K2Conn.Close();
            }
            return result;
        }

        /// <summary>
        /// Gets the current status of the workflow
        /// </summary>
        /// <param name="processinstanceId"></param>
        /// <returns></returns>
        public  string GetWorkflowStatus(int processinstanceId)
        {
            string Result = string.Empty;
            SourceCode.Workflow.Client.Connection K2Conn = new SourceCode.Workflow.Client.Connection();
            K2Conn.Open(servername);
            try
            {
                SourceCode.Workflow.Client.ProcessInstance K2Proc = K2Conn.OpenProcessInstance(processinstanceId);


                switch (K2Proc.Status1)
                {
                    case SourceCode.Workflow.Client.ProcessInstance.Status.Active:
                        {
                            Result = SourceCode.Workflow.Client.ProcessInstance.Status.Active.ToString();
                            break;
                        }

                    case SourceCode.Workflow.Client.ProcessInstance.Status.Completed:
                        {
                            Result = SourceCode.Workflow.Client.ProcessInstance.Status.Completed.ToString();
                            break;
                        }
                    case SourceCode.Workflow.Client.ProcessInstance.Status.Deleted:
                        {
                            Result = SourceCode.Workflow.Client.ProcessInstance.Status.Deleted.ToString();
                            break;
                        }
                    case SourceCode.Workflow.Client.ProcessInstance.Status.Error:
                        {
                            Result = SourceCode.Workflow.Client.ProcessInstance.Status.Error.ToString();
                            break;
                        }
                    case SourceCode.Workflow.Client.ProcessInstance.Status.New:
                        {
                            Result = SourceCode.Workflow.Client.ProcessInstance.Status.New.ToString();
                            break;
                        }
                    case SourceCode.Workflow.Client.ProcessInstance.Status.Running:
                        {
                            Result = SourceCode.Workflow.Client.ProcessInstance.Status.Running.ToString();
                            break;
                        }
                    case SourceCode.Workflow.Client.ProcessInstance.Status.Stopped:
                        {
                            Result = SourceCode.Workflow.Client.ProcessInstance.Status.Stopped.ToString();
                            break;
                        }


                }






            }
            catch (Exception ex)
            { Result = ex.Message; }
            finally
            {
                K2Conn.Close();
            }

            return Result;
        }

        /// <summary>
        /// Gets the process data fields
        /// </summary>
        /// <param name="processinstanceId"></param>
        /// <returns></returns>
        public  List<dataField> GetProcessDataFields(int processinstanceId)
        {
            List<dataField> list = new List<dataField>();
            SourceCode.Workflow.Client.Connection K2Conn = new SourceCode.Workflow.Client.Connection();
            K2Conn.Open(servername);
            try
            {
                SourceCode.Workflow.Client.ProcessInstance K2Proc = K2Conn.OpenProcessInstance(processinstanceId);
                foreach (SourceCode.Workflow.Client.DataField datafield in K2Proc.DataFields)
                {
                    list.Add(new dataField {

                        Name = datafield.Name,
                        Value = datafield.Value.ToString(),
                        valueType = datafield.ValueType.ToString()

                    });


                }
            }
            catch (Exception ex)
            {
                list.Add(new dataField
                {

                    Name = "Error",
                    Value = ex.Message.ToString()

                });

            }
            finally {
                K2Conn.Close();
            }

            return list;
        }


        public Boolean CompareActivities(int ProcessInstanceId, string Activities)
        {

            Boolean result = false;

            List<Activities> instAct = new List<Activities>();
            try
            {
                instAct = GetActivities(ProcessInstanceId.ToString());



                String[] activities = Activities.Split(';');
                int count = 0;
                int match = 0;
                foreach (var activity in activities)
                {
                    var instanceactivity = instAct.Where(p => p.ActivityName == activity);

                    foreach (var act in instanceactivity)
                    {
                        match++;
                    }

                    count++;
                }

                if (count == match)
                { result = true; }
            }
            catch (Exception ex)
            { }
            finally { }

            return result;

    }





        /// <summary>
        /// Gets the list of activities from process instance
        /// </summary>
        /// <param name="processinstanceId"></param>
        /// <returns></returns>
        public  List<Activities> GetActivities(string processinstanceId)

        {

            List<Activities> list = new List<Activities>();


            SourceCode.Hosting.Client.BaseAPI.SCConnectionStringBuilder hostServerConnectionString = new SourceCode.Hosting.Client.BaseAPI.SCConnectionStringBuilder();
            hostServerConnectionString.Host = servername;
            hostServerConnectionString.Port = 5555;
            hostServerConnectionString.IsPrimaryLogin = true;
            hostServerConnectionString.Integrated = true;

            SourceCode.SmartObjects.Client.SmartObjectClientServer serverName = new SourceCode.SmartObjects.Client.SmartObjectClientServer();
            serverName.CreateConnection();
            serverName.Connection.Open(hostServerConnectionString.ToString());

            try
            {

                SourceCode.SmartObjects.Client.SmartObject smartObject = serverName.GetSmartObject("Activity_Instance");
                smartObject.MethodToExecute = "List";
                smartObject.Properties["ProcessInstanceID"].Value = processinstanceId;
                SourceCode.SmartObjects.Client.SmartObjectList smoList = serverName.ExecuteList(smartObject);
                foreach (SourceCode.SmartObjects.Client.SmartObject item in smoList.SmartObjectsList)
                {

                    int ProcInstId = 0;
                    int ActInstId = 0;
                    int.TryParse(item.Properties["ProcessInstanceID"].Value, out ProcInstId);
                    int.TryParse(item.Properties["ActivityInstanceID"].Value, out ActInstId);

                    DateTime startDate = DateTime.Today;
                    DateTime finishDate = DateTime.Today;
                    DateTime.TryParse(item.Properties["StartDate"].Value, out startDate);
                    DateTime.TryParse(item.Properties["FinishDate"].Value, out finishDate);

                    list.Add(new Activities
                    {
                        ProcessInstanceId = ProcInstId,
                        ActivityInstanceId = ActInstId,
                        ActivityName = item.Properties["ActivityName"].Value,
                        Status = item.Properties["Status"].Value,
                        StartDate = startDate,
                        FinishDate = finishDate

                    });
                }

            }


            catch (Exception ex)
            {
                list.Add(new Activities
                {
                    ProcessInstanceId = 0,
                    ActivityInstanceId = 0,
                    ActivityName = ex.Message,
                    Status = "Error",
                    StartDate = DateTime.Today,
                    FinishDate = DateTime.Today

                });

            }
            finally {

                serverName.Connection.Close();
            }

            return list;
        }

        /// <summary>
        /// Gets a list of Events for an activity
        /// </summary>
        /// <param name="activityid"></param>
        /// <returns></returns>
        public  List<Events> GetEvents(string activityid)
        {
            List<Events> list = new List<Events>();


            SourceCode.Hosting.Client.BaseAPI.SCConnectionStringBuilder hostServerConnectionString = new SourceCode.Hosting.Client.BaseAPI.SCConnectionStringBuilder();
            hostServerConnectionString.Host = servername;
            hostServerConnectionString.Port = 5555;
            hostServerConnectionString.IsPrimaryLogin = true;
            hostServerConnectionString.Integrated = true;

            SourceCode.SmartObjects.Client.SmartObjectClientServer serverName = new SourceCode.SmartObjects.Client.SmartObjectClientServer();
            serverName.CreateConnection();
            serverName.Connection.Open(hostServerConnectionString.ToString());

            try
            {

                SourceCode.SmartObjects.Client.SmartObject smartObject = serverName.GetSmartObject("Event_Instance");
                smartObject.MethodToExecute = "List";
                smartObject.Properties["ActivityInstanceID"].Value = activityid;
                SourceCode.SmartObjects.Client.SmartObjectList smoList = serverName.ExecuteList(smartObject);
                foreach (SourceCode.SmartObjects.Client.SmartObject item in smoList.SmartObjectsList)
                {

                    int ProcInstId = 0;
                    int ActInstId = 0;
                    int.TryParse(item.Properties["ProcessInstanceID"].Value, out ProcInstId);
                    int.TryParse(item.Properties["ActivityInstanceID"].Value, out ActInstId);

                    DateTime startDate = DateTime.Today;
                    DateTime finishDate = DateTime.Today;
                    DateTime.TryParse(item.Properties["StartDate"].Value, out startDate);
                    DateTime.TryParse(item.Properties["FinishDate"].Value, out finishDate);

                    list.Add(new Events
                    {
                        ProcessInstanceId = ProcInstId,
                        ActivityInstanceId = ActInstId,
                        EventName = item.Properties["EventName"].Value,
                        Status = item.Properties["Status"].Value,
                        StartDate = startDate,
                        FinishDate = finishDate,
                        Destination = item.Properties["Destination"].Value

                    });
                }

            }


            catch (Exception ex)
            {
                list.Add(new Events
                {
                    ProcessInstanceId = 0,
                    ActivityInstanceId = 0,
                    EventName = ex.Message,
                    Status = "Error",
                    StartDate = DateTime.Today,
                    FinishDate = DateTime.Today

                });

            }
            finally
            {

                serverName.Connection.Close();
            }

            return list;


        }

        public  void IsErrors(int processinstanceId, string workflowname)
        {







        }





        /// <summary>
        /// Gets all the activities that are in a workflow
        /// </summary>
        /// <param name="ProcId"></param>
        /// <returns></returns>
        public  List<ActivityDesign> GetWorkflowActivities(int ProcId)
        {
            List<ActivityDesign> list = new List<ActivityDesign>();
            SourceCode.Workflow.Management.WorkflowManagementServer workflowServer = new SourceCode.Workflow.Management.WorkflowManagementServer(servername, 5555);
           workflowServer.Open();
            try
            {
                
                foreach (SourceCode.Workflow.Management.Activity activity in workflowServer.GetProcActivities( ProcId))
                {
                    list.Add(new ActivityDesign
                    {

                        ID = activity.ID,
                        Name = activity.Name,
                        Description = activity.Description

                    });




                }
            }
            catch (Exception ex)
            {
                list.Add(new ActivityDesign
                {

                    ID = 0,
                    Name = "Error",
                    Description = ex.Message
                });
            }
            finally
            {
                workflowServer.Connection.Close();
            }
            return list;

        }

        /// <summary>
        /// Gets a list of all the workflows
        /// </summary>
        /// <returns></returns>
        public  List<Workflow> GetProcesses()
        {
            List<Workflow> list = new List<Workflow>();

            SourceCode.Workflow.Management.WorkflowManagementServer workflowServer = new SourceCode.Workflow.Management.WorkflowManagementServer(servername,5555);
             workflowServer.Open();
            try
            {
                SourceCode.Workflow.Management.Criteria.ProcessCriteriaFilter filter = new SourceCode.Workflow.Management.Criteria.ProcessCriteriaFilter();
                SourceCode.Workflow.Management.Processes processes = workflowServer.GetProcesses(filter);
                foreach (SourceCode.Workflow.Management.Process process in processes)
                {
                    if (process.DefaultVersion == true)
                    {
                        list.Add(
                            new Workflow
                            {
                                ID = process.ProcID,
                                Name = process.FullName
                              

                            }

                        );
                    }
                }
            }
            catch (Exception ex)
            {
                list.Add(
                      new Workflow
                      {
                          ID = 0,
                          Name = ex.Message

                      }

                  );
            }
            finally
            {
                workflowServer.Connection.Close();
            }
            return list;
        }
    }

}

