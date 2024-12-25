using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomaGames.GameDoctor.Core
{
    /// <summary>
    /// Describe a specific issue in the project.
    /// IIssues are usually generated by IChecks.
    /// An IIssue defines a way to fix itself. 
    /// </summary>
    public interface IIssue
    {
        /// <summary>
        /// When the fix is executed.
        /// Gives back the issue and a boolean value saying if the fix process worked as expected.
        /// </summary>
        event System.Action<IIssue,bool> OnFixExecuted;
        /// <summary>
        /// Unique name for the issue.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Short description of the issue, this should be displayed by default in the issue inspector.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// The inspector drawing function.
        /// </summary>
        void Draw();
        AutomationType AutomationType { get; }
        Priority Priority { get; }
        /// <summary>
        /// Launches the fixing process, either automatic or interactive.
        /// Returns true if the Fix process worked as expected, false otherwise.
        /// </summary>
        Task<bool> Fix();
    }
}