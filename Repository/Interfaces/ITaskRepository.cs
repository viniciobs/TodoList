using Repository.DTOs.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
	internal interface ITaskRepository
	{
		public Task AssignTask(AssingTaskData data);
	}
}