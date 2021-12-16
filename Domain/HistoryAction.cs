namespace Domains
{
	public enum HistoryAction
	{
		Authenticated = 0,
		PasswordChanged = 1,
		DeletedAccount = 2,
		ActivatedAccount = 3,
		DeactivatedAccount = 4,
		ListedUsers = 5,
		AlteredUserRole = 6,
		AssignedTask = 7,
		FinishedTask = 8,
		ReopenedTask = 9,
		ListedTasks = 10,
		AddedCommentToTask = 11,
		ListedTaskComments = 12
	}
}
