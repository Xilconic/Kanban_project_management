// Copyright (c) 2020 S.L. des Bouvrie
//
// This file is part of 'Kanban Project Management App'.
//
// Kanban Project Management App is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Kanban Project Management App is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Kanban Project Management App.  If not, see https://www.gnu.org/licenses/.
using System;
using System.Collections.Generic;
using System.Linq;

namespace KanbanProjectManagementApp.Domain
{
    public class Roadmap
    {
        private readonly Project[] projects;

        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="projects"/> either
        /// <list type="bullet">
        /// <item>has no elements;</item>
        /// <item>contains a null element;</item>
        /// <item>contains a <see cref="Project"/> without work to be done;</item>
        /// <item>doesn't have unique names for all <see cref="Project"/> instances.</item>
        /// </list>
        /// </exception>
        public Roadmap(IEnumerable<Project> projects)
        {
            this.projects = projects?.ToArray() ?? throw new ArgumentNullException(nameof(projects));
            ValidateProjects(this.projects);
        }

        /// <remarks>
        /// All <see cref="Project"/> instances in this collection are guaranteed to have a unique <see cref="Project.Name"/>.
        /// </remarks>
        public IReadOnlyList<Project> Projects => projects;

        public bool HasWorkToBeCompleted => projects.Any(p => p.HasWorkToBeCompleted);

        public double TotalOfWorkRemaining => projects.Sum(p => p.NumberOfWorkItemsRemaining);

        public IReadOnlyList<Project> GetCurrentAvailableProjectsThatHaveWorkToBeCompleted() =>
            projects.Where(p => p.HasWorkToBeCompleted).ToArray();

        private static void ValidateProjects(Project[] projects)
        {
            if (projects.Length == 0)
            {
                throw new ArgumentException("Roadmap should contain at least one project.", nameof(projects));
            }

            var projectNames = new HashSet<string>();
            foreach (Project p in projects)
            {
                if (p is null)
                {
                    throw new ArgumentException("Sequence of projects for roadmap cannot contain null elements.", nameof(projects));
                }
                else if (!p.HasWorkToBeCompleted)
                {
                    throw new ArgumentException("Roadmap should contain only out of projects that has work to be completed.", nameof(projects));
                }

                if(!projectNames.Add(p.Name))
                {
                    throw new ArgumentException("Sequence of projects for roadmap cannot contain multiple projects with the same name.", nameof(projects));
                }
            }
        }
    }
}
