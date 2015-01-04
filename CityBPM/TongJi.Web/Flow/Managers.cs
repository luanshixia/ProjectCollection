using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TongJi.Web.Models;
using System.Data.Entity;

namespace TongJi.Web.Flow
{
    public static class WorkflowManager
    {
        public static Workflow GetDbRecord(Guid id)
        {
            using (BasicWebContext context = new BasicWebContext())
            {
                return context.Workflows.AsNoTracking().Single(x => x.ID == id);
            }
        }

        public static void New(Guid id, string name, List<FlowNode> nodes)
        {
            using (BasicWebContext context = new BasicWebContext())
            {
                Workflow wf = new Workflow { ID = id, Name = name, CreateTime = DateTime.Now, ModifyTime = DateTime.Now };
                wf.Nodes = nodes;
                context.Workflows.Add(wf);
                context.SaveChanges();
            }
        }

        public static void New(string name, WorkflowJsonObject flow)
        {
            var nodes = new List<FlowNode>();
            foreach (var jsonNode in flow.nodes)
            {
                FlowNode node = new FlowNode(Guid.NewGuid(), jsonNode.name, jsonNode.user, jsonNode.role, jsonNode.xpos, jsonNode.ypos);
                nodes.Add(node);
            }
            foreach (var jsonConn in flow.connections)
            {
                nodes[jsonConn.from].ConnectTo(nodes[jsonConn.to]);
            }
            WorkflowManager.New(Guid.NewGuid(), name, nodes);
        }

        public static void UpdateFromJson(WorkflowJsonObject flow)
        {
            using (var context = new TongJi.Web.Models.BasicWebContext())
            {
                foreach (var node in flow.nodes)
                {
                    var flowNode = context.FlowNodes.Find(node.id);
                    flowNode.Name = node.name;
                    flowNode.FlowChartPositionX = node.xpos;
                    flowNode.FlowChartPositionY = node.ypos;
                    flowNode.AllowedUsers = node.user;
                    flowNode.AllowedRoles = node.role;
                }
                context.SaveChanges();
            }
        }

        public static void EditGroup(Guid id, string group)
        {
            using (var context = new BasicWebContext())
            {
                var data = context.Workflows.Single(x => x.ID == id);
                data.Group = group;
                context.SaveChanges();
            }
        }

        public static int NewInstance(Guid id, string instName, string number, string comment)
        {
            using (BasicWebContext context = new BasicWebContext())
            {
                var startNodeID = context.FlowNodes.Where(x => x.WorkflowID == id).Include("FromNodes").First(x => x.FromNodes.Count == 0).ID;
                FlowInstance inst = new FlowInstance(instName, id, startNodeID) { Number = number, Comment = comment };
                context.FlowInstances.Add(inst);
                context.SaveChanges();
                return context.FlowInstances.Max(x => x.ID);
            }
        }

        public static List<TodoListItem> GetTodoList(string user)
        {
            using (BasicWebContext context = new BasicWebContext())
            {
                if (string.IsNullOrEmpty(user))
                {
                    return new List<TodoListItem>();
                }
                var insts = context.FlowInstances.Where(x => !x.Completed).AsEnumerable().Where(x => FlowNodeManager.IsUserAllowed(x.CurrentNodeID, user)).ToList();
                return insts.Select((x, i) =>
                {
                    TodoListItem item = new TodoListItem();
                    item.ID = i + 1;
                    item.FlowName = context.Workflows.Single(y => y.ID == x.WorkflowID).Name;
                    item.InstName = x.Name;
                    item.InstNumber = x.Number; // newly 20130708
                    item.InstComment = x.Comment; // newly 20130708
                    item.CurrentNode = context.FlowNodes.Single(y => y.ID == x.CurrentNodeID).Name;
                    item.StartTime = x.StartTime;
                    item.EndTime = x.EndTime; // newly 20130708
                    item.InstID = x.ID;
                    return item;
                }).ToList();
            }
        }

        public static List<TodoListItem> GetCompletedDoneList(string user)
        {
            using (BasicWebContext context = new BasicWebContext())
            {
                if (string.IsNullOrEmpty(user))
                {
                    return new List<TodoListItem>();
                }
                var instIDs = context.FlowInstanceActions.Where(x => x.Performer == user).Select(x => x.FlowInstanceID).Distinct(); //.ToList();
                var insts = context.FlowInstances.Where(x => x.Completed)
                                   .Join(instIDs, x => x.ID, y => y, (x, y) => x)
                                   .OrderByDescending(x => x.EndTime);
                var query = from inst in insts
                            from flow in context.Workflows
                            where flow.ID == inst.WorkflowID
                            select new TodoListItem
                            {
                                FlowName = flow.Name,
                                InstName = inst.Name,
                                InstNumber = inst.Number,
                                InstComment = inst.Comment,
                                StartTime = inst.StartTime,
                                EndTime = inst.EndTime,
                                InstID = inst.ID
                            };
                var result = query.ToList();
                result.ForEach((r, i) => r.ID = i + 1);
                return result;
            }
        }

        public static List<TodoListItem> GetActiveDoneList(string user)
        {
            using (BasicWebContext context = new BasicWebContext())
            {
                if (string.IsNullOrEmpty(user))
                {
                    return new List<TodoListItem>();
                }
                var instIDs = context.FlowInstanceActions.Where(x => x.Performer == user).Select(x => x.FlowInstanceID).Distinct(); //.ToList();
                var insts = context.FlowInstances.Where(x => !x.Completed)
                                   .Join(instIDs, x => x.ID, y => y, (x, y) => x); // && instIDs.Contains(x.ID));
                var query = from inst in insts
                            from flow in context.Workflows
                            from node in context.FlowNodes
                            where flow.ID == inst.WorkflowID && node.ID == inst.CurrentNodeID
                            select new TodoListItem
                            {
                                FlowName = flow.Name,
                                InstName = inst.Name,
                                InstNumber = inst.Number,
                                InstComment = inst.Comment,
                                StartTime = inst.StartTime,
                                EndTime = inst.EndTime, // newly 20130708
                                CurrentNode = node.Name,
                                InstID = inst.ID
                            };
                var result = query.ToList();
                result.ForEach((r, i) => r.ID = i + 1);
                return result;
            }
        }

        public static List<TrackBackListItem> GetActionHistory(int num = 10)
        {
            using (BasicWebContext context = new BasicWebContext())
            {
                var actions = context.FlowInstanceActions.OrderByDescending(x => x.PerformTime).Take(num);
                var query = from action in actions
                            from inst in context.FlowInstances
                            from flow in context.Workflows
                            from fromNode in context.FlowNodes
                            from toNode in context.FlowNodes
                            where inst.ID == action.FlowInstanceID && flow.ID == inst.WorkflowID && fromNode.ID == action.FromNodeID && toNode.ID == action.ToNodeID
                            select new TrackBackListItem
                            {
                                FlowName = flow.Name,
                                InstName = inst.Name,
                                InstNumber = inst.Number,
                                FromNodeName = fromNode.Name,
                                ToNodeName = toNode.Name,
                                PerformTime = action.PerformTime,
                                Performer = action.Performer
                            };
                var result = query.ToList();
                result.ForEach((r, i) => r.ID = i + 1);
                return result;
            }
        }

        public static WorkflowJsonObject GetJsonObject(Guid id)
        {
            using (BasicWebContext context = new BasicWebContext())
            {
                WorkflowJsonObject json = new WorkflowJsonObject();
                var nodes = context.FlowNodes.Include("ToNodes").Where(x => x.WorkflowID == id).ToList();
                json.id = id.ToString();
                json.nodes = nodes.Select(x => new FlowNodeJsonObject { id = x.ID.ToString(), name = x.Name, user = x.AllowedUsers, role = x.AllowedRoles, xpos = x.FlowChartPositionX, ypos = x.FlowChartPositionY, status = "" }).ToList();
                json.connections = nodes.SelectMany(x => x.ToNodes.Select(y => new NodeConnectionJsonObject { from = nodes.IndexOf(x), to = nodes.IndexOf(y), label = GetLabel(x, y) })).ToList();
                json.labels = context.FlowChartTextLabels.Where(x => x.WorkflowID == id).Select(x => new TextLabelJsonObject { text = x.Text, xpos = x.PositionX, ypos = x.PositionY, centerAligned = x.CenterAligned, fontFamily = x.FontFamily, fontSize = x.FontSize }).ToList();
                return json;
            }
        }

        private static string GetLabel(FlowNode from, FlowNode to)
        {
            if (string.IsNullOrEmpty(from.ToLabels))
            {
                return string.Empty;
            }
            var labels = from.ToLabels.Split('|');
            int index = from.ToNodes.IndexOf(to);
            if (index >= labels.Length)
            {
                return string.Empty;
            }
            return labels[from.ToNodes.IndexOf(to)];
        }

        // done: 重写以下2个函数查询

        public static List<FlowListItem> GetFlowList() // mod 20130708
        {
            using (BasicWebContext context = new BasicWebContext())
            {
                var query = from flow in context.Workflows
                            join node in context.FlowNodes on flow.ID equals node.WorkflowID into nodes
                            join inst in context.FlowInstances on flow.ID equals inst.WorkflowID into insts
                            select new
                            {
                                ID = flow.ID,
                                FlowName = flow.Name,
                                Group = flow.Group,
                                CreateTime = flow.CreateTime,
                                NodeCount = nodes.Count(),
                                InstanceCount = insts.Count(),
                                ActiveInstanceCount = insts.Where(x => !x.Completed).Count()
                            };
                var result = query.ToList().Select(x => new FlowListItem
                {
                    ID = x.ID.ToString().ToUpper(),
                    FlowName = x.FlowName,
                    Group = x.Group,
                    CreateTime = x.CreateTime,
                    NodeCount = x.NodeCount,
                    InstanceCount = x.InstanceCount,
                    ActiveInstanceCount = x.ActiveInstanceCount
                }).ToList();
                return result;
            }
        }

        public static List<FlowListItem> GetFlowList(string user)
        {
            using (var context = new BasicWebContext())
            {
                var workflows = context.Workflows.ToList()
                    .Where(x => context.FlowNodes.Where(n => n.WorkflowID == x.ID)
                        .First(n => n.FromNodes.Count == 0)
                        .IsUserAllowed(user));
                var nodes = context.FlowNodes.ToList();
                var insts = context.FlowInstances.ToList();

                return workflows.ToList().Select(x =>
                {
                    var item = new FlowListItem();
                    item.ID = x.ID.ToString().ToUpper();
                    item.FlowName = x.Name;
                    item.Group = x.Group;
                    item.CreateTime = x.CreateTime;
                    item.NodeCount = nodes.Count(y => y.WorkflowID == x.ID);
                    item.InstanceCount = insts.Count(y => y.WorkflowID == x.ID);
                    item.ActiveInstanceCount = insts.Where(y => !y.Completed).Count(y => y.WorkflowID == x.ID);
                    return item;
                }).ToList();
            }
        }

        public static List<NodeListItem> GetNodeList(Guid id)
        {
            using (var context = new BasicWebContext())
            {
                return context.FlowNodes.Where(x => x.WorkflowID == id).Include("FromNodes").Include("ToNodes").ToList().Select(x =>
                {
                    var item = new NodeListItem();
                    item.ID = x.ID.ToString().ToUpper();
                    item.NodeName = x.Name;
                    item.FromNodes = string.Join(", ", x.FromNodes.Select(y => y.Name));
                    item.ToNodes = string.Join(", ", x.ToNodes.Select(y => y.Name));
                    item.Users = x.AllowedUsers;
                    item.Roles = x.AllowedRoles;
                    return item;
                }).ToList();
            }
        }

        public static void AddTextLabel(string text, Guid flowID, double x, double y)
        {
            FlowChartTextLabel label = new FlowChartTextLabel { Text = text, WorkflowID = flowID, PositionX = x, PositionY = y };
            using (BasicWebContext context = new BasicWebContext())
            {
                context.FlowChartTextLabels.Add(label);
                context.SaveChanges();
            }
        }
    }

    public static class FlowNodeManager
    {
        public static void ConnectTo(this FlowNode node0, FlowNode node)
        {
            node0.ToNodes.Add(node);
        }

        public static void SetDisplayPage(Guid id, string page)
        {
            using (BasicWebContext context = new BasicWebContext())
            {
                FlowNode node = context.FlowNodes.Find(id);
                node.DisplayPage = page;
                context.SaveChanges();
            }
        }

        internal static bool IsUserAllowed(this FlowNode node, string user)
        {
            if (string.IsNullOrEmpty(user))
            {
                return false;
            }
            if (node.AllowedUsers == "*" || node.AllowedRoles == "*")
            {
                return true;
            }
            else
            {
                var users = node.AllowedUsers.Split(',');
                var roles = node.AllowedRoles.Split(',');
                if (users.Contains(user) || roles.Any(x => (!string.IsNullOrEmpty(x)) && System.Web.Security.Roles.IsUserInRole(user, x)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool IsUserAllowed(Guid id, string user) // mod 20130710
        {
            var node = FlowNodeManager.GetDbRecord(id);
            return IsUserAllowed(node, user);
        }

        public static void SetRoles(Guid id, string roles)
        {
            using (BasicWebContext context = new BasicWebContext())
            {
                FlowNode node = context.FlowNodes.Find(id);
                node.AllowedRoles = roles;
                context.SaveChanges();
            }
        }

        public static void AddUser(Guid id, string user)
        {
            using (BasicWebContext context = new BasicWebContext())
            {
                FlowNode node = context.FlowNodes.Find(id);
                var users = string.IsNullOrEmpty(node.AllowedUsers) ? new List<string>() : node.AllowedUsers.Split(',').ToList();
                users.Add(user);
                node.AllowedUsers = string.Join(",", users);
                context.SaveChanges();
            }
        }

        public static void DeleteUser(Guid id, string user)
        {
            using (BasicWebContext context = new BasicWebContext())
            {
                FlowNode node = context.FlowNodes.Find(id);
                var users = string.IsNullOrEmpty(node.AllowedUsers) ? new List<string>() : node.AllowedUsers.Split(',').ToList();
                users.Remove(user);
                node.AllowedUsers = string.Join(",", users);
                context.SaveChanges();
            }
        }

        public static void AddRole(Guid id, string role)
        {
            using (BasicWebContext context = new BasicWebContext())
            {
                FlowNode node = context.FlowNodes.Find(id);
                var roles = string.IsNullOrEmpty(node.AllowedRoles) ? new List<string>() : node.AllowedRoles.Split(',').ToList();
                roles.Add(role);
                node.AllowedRoles = string.Join(",", roles);
                context.SaveChanges();
            }
        }

        public static void DeleteRole(Guid id, string role)
        {
            using (BasicWebContext context = new BasicWebContext())
            {
                FlowNode node = context.FlowNodes.Find(id);
                var roles = string.IsNullOrEmpty(node.AllowedRoles) ? new List<string>() : node.AllowedRoles.Split(',').ToList();
                roles.Remove(role);
                node.AllowedRoles = string.Join(",", roles);
                context.SaveChanges();
            }
        }

        public static FlowNode GetDbRecord(Guid id)
        {
            using (BasicWebContext context = new BasicWebContext())
            {
                return context.FlowNodes.Find(id);
            }
        }
    }

    public static class FlowInstanceManager
    {
        public static bool GoOn(int id, string performer, string toNodeName = null)
        {
            using (BasicWebContext context = new BasicWebContext())
            {
                FlowInstance inst = context.FlowInstances.Find(id);
                FlowNode currentNode = context.FlowNodes.Include("ToNodes").Single(x => x.ID == inst.CurrentNodeID);
                if (currentNode.ToNodes.Count > 0)
                {
                    var fromNodeID = inst.CurrentNodeID;
                    var toNodeID = currentNode.ToNodes[0].ID;
                    if (!string.IsNullOrEmpty(toNodeName))
                    {
                        if (currentNode.ToNodes.Any(x => x.Name == toNodeName))
                        {
                            toNodeID = currentNode.ToNodes.First(x => x.Name == toNodeName).ID;
                        }
                    }
                    inst.CurrentNodeID = toNodeID;
                    inst.EndTime = DateTime.Now; // newly 20130708
                    FlowInstanceAction action = new FlowInstanceAction(id, fromNodeID, toNodeID, performer);
                    context.FlowInstanceActions.Add(action);
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    inst.Completed = true;
                    inst.EndTime = DateTime.Now;
                    context.SaveChanges();
                }
                return false;
            }
        }

        public static void GoBack(int id, string performer) // newly 20130710 只支持回退一步，在最后两步之间来回跳转。
        {
            using (BasicWebContext context = new BasicWebContext())
            {
                FlowInstance inst = context.FlowInstances.Find(id);
                FlowInstanceAction action = context.FlowInstanceActions.Where(x => x.FlowInstanceID == id).ToList().LastOrDefault();
                if (action != null)
                {
                    var currentID = action.ToNodeID;
                    var backToID = action.FromNodeID;
                    inst.CurrentNodeID = backToID;
                    inst.EndTime = DateTime.Now;
                    FlowInstanceAction newAction = new FlowInstanceAction(id, currentID, backToID, performer);
                    context.FlowInstanceActions.Add(newAction);
                    context.SaveChanges();
                }
            }
        }

        public static void SetCurrentNode(int id, Guid nodeId)
        {
            using (BasicWebContext context = new BasicWebContext())
            {
                FlowInstance inst = context.FlowInstances.Find(id);
                inst.CurrentNodeID = nodeId;
                context.SaveChanges();
            }
        }

        public static List<TrackBackListItem> TrackBack(int id)
        {
            using (BasicWebContext context = new BasicWebContext())
            {
                var inst = context.FlowInstances.Find(id);
                string flowName = context.Workflows.Single(x => x.ID == inst.WorkflowID).Name;
                var actions = context.FlowInstanceActions.Where(x => x.FlowInstanceID == id).OrderBy(x => x.PerformTime);
                var query = from action in actions
                            join fromNode in context.FlowNodes on action.FromNodeID equals fromNode.ID
                            join toNode in context.FlowNodes on action.ToNodeID equals toNode.ID
                            select new TrackBackListItem
                            {
                                FlowName = flowName,
                                InstName = inst.Name,
                                InstNumber = inst.Number,
                                FromNodeName = fromNode.Name,
                                ToNodeName = toNode.Name,
                                PerformTime = action.PerformTime,
                                Performer = action.Performer
                            };
                var result = query.ToList();
                result.ForEach((r, i) => r.ID = i + 1);
                return result;
            }
        }

        public static List<TrackBackListItem> UserAction(string user)
        {
            using (BasicWebContext context = new BasicWebContext())
            {
                var actions = context.FlowInstanceActions.Where(x => x.Performer == user).OrderByDescending(x => x.PerformTime);
                var query = from action in actions
                            from inst in context.FlowInstances
                            from flow in context.Workflows
                            join fromNode in context.FlowNodes on action.FromNodeID equals fromNode.ID
                            join toNode in context.FlowNodes on action.ToNodeID equals toNode.ID
                            where inst.ID == action.FlowInstanceID && flow.ID == inst.WorkflowID
                            select new TrackBackListItem
                            {
                                FlowName = flow.Name,
                                InstName = inst.Name,
                                InstNumber = inst.Number,
                                FromNodeName = fromNode.Name,
                                ToNodeName = toNode.Name,
                                PerformTime = action.PerformTime,
                                Performer = action.Performer
                            };
                var result = query.ToList();
                result.ForEach((r, i) => r.ID = i + 1);
                return result;
            }
        }

        public static FlowInstance GetDbRecord(int id)
        {
            using (BasicWebContext context = new BasicWebContext())
            {
                return context.FlowInstances.AsNoTracking().Single(x => x.ID == id);
            }
        }

        public static List<FlowInstance> GetAllRecords()
        {
            using (BasicWebContext context = new BasicWebContext())
            {
                return context.FlowInstances.ToList();
            }
        }

        public static List<FlowInstance> Query(System.Linq.Expressions.Expression<Func<FlowInstance, bool>> condition)
        {
            using (BasicWebContext context = new BasicWebContext())
            {
                return context.FlowInstances.Where(condition).ToList();
            }
        }

        public static WorkflowJsonObject GetJsonObject(int id)
        {
            using (BasicWebContext context = new BasicWebContext())
            {
                WorkflowJsonObject json = new WorkflowJsonObject();
                var inst = context.FlowInstances.Find(id);
                var nodes = context.FlowNodes.Include("ToNodes").Where(x => x.WorkflowID == inst.WorkflowID).ToList();
                var actions = context.FlowInstanceActions.Where(x => x.FlowInstanceID == id).OrderBy(x => x.PerformTime).ToList();
                json.nodes = nodes.Select(x =>
                {
                    string status = "unreached";
                    if (inst.CurrentNodeID == x.ID)
                    {
                        status = "current";
                    }
                    else if (actions.Any(y => y.FromNodeID == x.ID))
                    {
                        status = "passed";
                    }
                    return new FlowNodeJsonObject { name = x.Name, user = x.AllowedUsers, role = x.AllowedRoles, xpos = x.FlowChartPositionX, ypos = x.FlowChartPositionY, status = status };
                }).ToList();
                json.connections = nodes.SelectMany(x => x.ToNodes.Select(y => new NodeConnectionJsonObject { from = nodes.IndexOf(x), to = nodes.IndexOf(y), label = GetLabel(x, y) })).ToList();
                json.labels = context.FlowChartTextLabels.Where(x => x.WorkflowID == inst.WorkflowID).Select(x => new TextLabelJsonObject { text = x.Text, xpos = x.PositionX, ypos = x.PositionY, centerAligned = x.CenterAligned, fontFamily = x.FontFamily, fontSize = x.FontSize }).ToList();
                return json;
            }
        }

        private static string GetLabel(FlowNode from, FlowNode to)
        {
            if (string.IsNullOrEmpty(from.ToLabels))
            {
                return string.Empty;
            }
            var labels = from.ToLabels.Split('|');
            int index = from.ToNodes.IndexOf(to);
            if (index >= labels.Length)
            {
                return string.Empty;
            }
            return labels[from.ToNodes.IndexOf(to)];
        }

        //public static string GetFlowInstNumber(int instId, string postfix = "")
        //{
        //    string prefix = "DYGH";
        //    string date = DateTime.Now.ToShortDateString().Where(x => char.IsDigit(x)).Aggregate(string.Empty, (s, c) => s + c);
        //    string id = (instId % 10000).ToString().PadLeft(4, '0');
        //    return string.Format("{0}{1}{2}{3}", prefix, date, id, postfix);
        //}

        public static List<TodoListItem> Search(int page = 0, int pageSize = 30, string keyword = null, Guid? flowID = null, DateTime? fromTime = null, DateTime? toTime = null)
        {
            using (BasicWebContext context = new BasicWebContext())
            {
                var insts = context.FlowInstances.OrderByDescending(x => x.StartTime).AsQueryable();
                if (keyword != null)
                {
                    insts = insts.Where(x => x.Name.Contains(keyword) || x.Number.Contains(keyword) || x.Comment.Contains(keyword));
                }
                if (flowID != null)
                {
                    insts = insts.Where(x => x.WorkflowID == flowID);
                }
                if (fromTime != null)
                {
                    insts = insts.Where(x => x.StartTime >= fromTime);
                }
                if (toTime != null)
                {
                    insts = insts.Where(x => x.StartTime <= toTime);
                }
                var query = from inst in insts
                            from node in context.FlowNodes
                            from flow in context.Workflows
                            where inst.WorkflowID == flow.ID && inst.CurrentNodeID == node.ID
                            select new TodoListItem
                            {
                                FlowName = flow.Name,
                                InstName = inst.Name,
                                CurrentNode = node.Name,
                                StartTime = inst.StartTime,
                                EndTime = inst.EndTime,
                                InstID = inst.ID
                            };
                var result = query.Skip(page * pageSize).Take(pageSize).ToList();
                result.ForEach((r, i) => r.ID = i + 1);
                return result;
            }
        }

        public static bool CanBack(int instId)
        {
            using (var context = new BasicWebContext())
            {
                var b = false;
                var records = context.FlowInstanceActions.Where(x => x.FlowInstanceID == instId).ToArray();
                var count = records.Count();
                if (count > 2)
                {
                    if (records[count - 2].FromNodeID == records[count - 1].ToNodeID)
                    {
                        b = true;
                    }
                }
                return b;
            }
        }
    }

    public static class FlowInstanceActionManager // newly 20130605
    {
        public static FlowInstanceAction GetDbRecord(int id)
        {
            using (BasicWebContext context = new BasicWebContext())
            {
                return context.FlowInstanceActions.AsNoTracking().Single(x => x.ID == id);
            }
        }

        public static bool IsBack(int instId)
        {
            using (var context = new BasicWebContext())
            {
                var b = false;
                var records = context.FlowInstanceActions.Where(x => x.FlowInstanceID == instId).ToArray();
                var count = records.Count();
                if (count > 2)
                {
                    if (records[count - 2].FromNodeID == records[count - 1].ToNodeID)
                    {
                        b = true;
                    }
                }
                return b;
            }
        }

        public static List<TrackBackListItem> GetActionHistory(int page = 0, int pageSize = 30, int instID = 0, Guid? flowID = null, string user = null)
        {
            using (BasicWebContext context = new BasicWebContext())
            {
                var actions = context.FlowInstanceActions.OrderByDescending(x => x.PerformTime).AsQueryable();
                if (instID > 0)
                {
                    actions = actions.Where(x => x.FlowInstanceID == instID);
                }
                else if (flowID != null) // instID 和 flowID 只能用一个
                {
                    actions = actions.Join(context.FlowInstances.Where(x => x.WorkflowID == flowID),
                                           x => x.FlowInstanceID, y => y.ID, (x, y) => x);
                }
                if (user != null)
                {
                    actions = actions.Where(x => x.Performer == user);
                }
                var query = from action in actions
                            from inst in context.FlowInstances
                            from flow in context.Workflows
                            from fromNode in context.FlowNodes
                            from toNode in context.FlowNodes
                            where inst.ID == action.FlowInstanceID && flow.ID == inst.WorkflowID && fromNode.ID == action.FromNodeID && toNode.ID == action.ToNodeID
                            select new TrackBackListItem
                            {
                                FlowName = flow.Name,
                                InstName = inst.Name,
                                InstNumber = inst.Number,
                                FromNodeName = fromNode.Name,
                                ToNodeName = toNode.Name,
                                PerformTime = action.PerformTime,
                                Performer = action.Performer
                            };
                var result = query.Skip(page * pageSize).Take(pageSize).ToList();
                result.ForEach((r, i) => r.ID = i + 1);
                return result;
            }
        }
    }
}
