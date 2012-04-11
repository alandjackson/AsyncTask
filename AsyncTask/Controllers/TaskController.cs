using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Threading;

namespace AsyncTask.Controllers
{
    public class TaskController : Controller
    {
        static Dictionary<string, Task<string>> Tasks = new Dictionary<string, Task<string>>();

        public ActionResult Begin(FormCollection form)
        {
            string taskId = Guid.NewGuid().ToString();
            Tasks[taskId] = Task.Factory.StartNew<string>(() => RunTask(form));
            return Content(taskId);
        }

        public ActionResult GetStatus(string id)
        {
            if (!Tasks.ContainsKey(id))
                return Content("Unknown Task");
            if (Tasks[id].IsCompleted)
                return Content("Done");
            return Content("Running");
        }

        public ActionResult End(string id)
        {
            if (!Tasks.ContainsKey(id))
                return Content("Unknown Task");
            if (!Tasks[id].IsCompleted)
                Task.WaitAll(Tasks[id]);

            return Content(Tasks[id].Result);
        }

        private string RunTask(FormCollection form)
        {
            try
            {
                string name = form["Name"];
                string sleep = form["Sleep"];

                int secondsToSleep;
                if (!int.TryParse(sleep, out secondsToSleep) || secondsToSleep <= 0 || secondsToSleep > 5)
                    secondsToSleep = 3;

                Thread.Sleep(secondsToSleep * 1000);
                return name + " requested that I sleep for " + sleep + " seconds.";
            }
            catch (Exception e)
            {
                return "Error: " + e.Message;
            }
        }


    }
}
