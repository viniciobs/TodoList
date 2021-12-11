using Domains.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domains
{
	public partial class User
	{
		public partial class Task
		{
			#region Properties

			public Guid Id { get; private set; }
			public string Description { get; private set; }
			public DateTime CreatedAt { get; private set; }
			public DateTime? CompletedAt { get; private set; }
			public Guid CreatorUserId { get; private set; }
			public virtual User CreatorUser { get; private set; }
			public Guid TargetUserId { get; private set; }
			public virtual User TargetUser { get; private set; }

			[NotMapped]
			public ICollection<TaskComment> Comments { get; private set; }

			#endregion Properties

			#region Constructor

			private Task()
			{ }

			private Task(User creator, User target, string description)
			{
				if (creator == null) throw new MissingArgumentsException(nameof(creator));
				if (target == null) throw new MissingArgumentsException(nameof(target));
				if (string.IsNullOrEmpty(description)) throw new MissingArgumentsException(nameof(description));

				CreatorUser = creator;
				CreatorUserId = creator.Id;
				TargetUser = target;
				TargetUserId = target.Id;
				Description = description;
				CreatedAt = DateTime.UtcNow;

				Comments = new HashSet<TaskComment>();
			}

			#endregion Constructor

			#region Methods

			internal static Task New(User creator, User target, string description)
			{
				return new Task(creator, target, description);
			}

			public void Finish()
			{
				if (CompletedAt != null) throw new RuleException("Task is finished already");

				CompletedAt = DateTime.UtcNow;
			}

			public void Reopen()
			{
				if (CompletedAt == null) throw new RuleException("Task is not finished");

				CompletedAt = null;
			}

			public void AddComment(User creator, string text)
			{
				var comment = TaskComment.New(this, creator, text);

				Comments.Add(comment);
			}

			#endregion Methods
		}
	}
}