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
using KanbanProjectManagementApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KanbanProjectManagementApp.Application
{
    public class RoadmapConfiguration
    {
        private IReadOnlyCollection<ProjectConfiguration> configuredProjects;

        /// <exception cref="ArgumentException">Thrown when <paramref name="configuredProjects"/> is invalid.</exception>
        public RoadmapConfiguration(IEnumerable<ProjectConfiguration> configuredProjects)
        {
            ValidateProjects(configuredProjects);
            this.configuredProjects = configuredProjects.ToArray();
        }

        /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is invalid.</exception>
        public IReadOnlyCollection<ProjectConfiguration> Projects
        {
            get => configuredProjects;
            set
            {
                ValidateProjects(value);
                configuredProjects = value;
            }
        }

        public Roadmap ToWorkableRoadmap() => new Roadmap(configuredProjects.Select(prop => prop.ToWorkableProject()));

        /// <exception cref="ArgumentException">Thrown when <paramref name="configuredProjects"/> is invalid.</exception>
        private static void ValidateProjects(IEnumerable<ProjectConfiguration> configuredProjects)
        {
            if (configuredProjects is null) throw new ArgumentNullException(nameof(configuredProjects));

            var projectNames = new HashSet<string>();
            foreach (ProjectConfiguration p in configuredProjects)
            {
                if (p is null)
                {
                    throw new ArgumentException("Sequence of projects for roadmap cannot contain null elements.", nameof(configuredProjects));
                }

                if (!projectNames.Add(p.Name))
                {
                    throw new ArgumentException("Sequence of projects for roadmap cannot contain multiple projects with the same name.", nameof(configuredProjects));
                }
            }
            if (projectNames.Count == 0)
            {
                throw new ArgumentException("Roadmap should contain at least one project.", nameof(configuredProjects));
            }
        }
    }
}
