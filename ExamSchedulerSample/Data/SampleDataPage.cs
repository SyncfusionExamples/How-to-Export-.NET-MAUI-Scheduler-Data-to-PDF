namespace ExamSchedulerSample
{
    /// <summary>
    /// Identifies the academic program for which exam data is generated.
    /// </summary>
    public enum ProgramType
    {
        UG,
        PG,
        MBA
    }

    /// <summary>
    /// Provides deterministic data (halls, invigilators, courses) and utility methods for building complete exam schedules used by the app.
    /// </summary>
    public static class SampleDataPage
    {
        /// <summary>
        /// Returns the list of exam halls with identifiers, names, capacity, and display colors.
        /// </summary>
        public static List<Hall> GetHalls() => new()
        {
            new Hall { Id = "H1", Name = "Main Hall A",   Capacity = 120, Color = Colors.Orange },
            new Hall { Id = "H2", Name = "Main Hall B",   Capacity = 100, Color = Colors.MediumPurple },
            new Hall { Id = "H3", Name = "Annex 101",     Capacity =  60, Color = Colors.SteelBlue },
            new Hall { Id = "H4", Name = "Seminar Block", Capacity =  80, Color = Colors.Teal }
        };

        /// <summary>
        /// Returns the list of invigilators with identifiers, names, departments, and display colors.
        /// </summary>
        public static List<Invigilator> GetInvigilators() => new()
        {
            new Invigilator { Id = "T1", Name = "Dr. Anitha", Department = "CSE", Color = Colors.SeaGreen },
            new Invigilator { Id = "T2", Name = "Mr. Kumar",  Department = "ECE", Color = Colors.Teal },
            new Invigilator { Id = "T3", Name = "Ms. Roy",    Department = "EEE", Color = Colors.IndianRed },
            new Invigilator { Id = "T4", Name = "Dr. Varun",  Department = "IT",  Color = Colors.DarkCyan }
        };

        /// <summary>
        /// Returns a curated set of <b>UG</b> courses across CSE, ECE, and EEE.
        /// </summary>
        public static List<Course> GetUgCourses() => new()
        {
            // CSE (UG)
            new Course { Code = "CS101E", Title = "Data Structures",        Semester = 3, Department = "CSE" },
            new Course { Code = "CS204E", Title = "Operating Systems",      Semester = 4, Department = "CSE" },
            new Course { Code = "CS305E", Title = "Compiler Design",        Semester = 5, Department = "CSE" },
            new Course { Code = "CS312E", Title = "Database Systems",       Semester = 5, Department = "CSE" },
            new Course { Code = "CS350E", Title = "Computer Networks",      Semester = 6, Department = "CSE" },
            new Course { Code = "CS360E", Title = "Machine Learning",       Semester = 6, Department = "CSE" },

            // ECE (UG)
            new Course { Code = "EC202E", Title = "Signals & Systems",      Semester = 4, Department = "ECE" },
            new Course { Code = "EC310E", Title = "Embedded Systems",       Semester = 5, Department = "ECE" },
            new Course { Code = "EC315E", Title = "Digital Communication",  Semester = 5, Department = "ECE" },
            new Course { Code = "EC320E", Title = "VLSI Design",            Semester = 6, Department = "ECE" },
            new Course { Code = "EC330E", Title = "Microprocessors",        Semester = 6, Department = "ECE" },
            new Course { Code = "EC340E", Title = "Wireless Communication", Semester = 6, Department = "ECE" },

            // EEE (UG)
            new Course { Code = "EE201E", Title = "Circuit Theory",           Semester = 3, Department = "EEE" },
            new Course { Code = "EE305E", Title = "Power Electronics",        Semester = 5, Department = "EEE" },
            new Course { Code = "EE312E", Title = "Electrical Machines",      Semester = 5, Department = "EEE" },
            new Course { Code = "EE330E", Title = "Control Systems",          Semester = 6, Department = "EEE" },
            new Course { Code = "EE340E", Title = "High Voltage Engineering", Semester = 6, Department = "EEE" },
            new Course { Code = "EE350E", Title = "Renewable Energy Systems", Semester = 6, Department = "EEE" },
        };

        /// <summary>
        /// Returns a curated set of <b>PG</b> courses across CSE, ECE, and EEE.
        /// </summary>
        public static List<Course> GetPgCourses() => new()
        {
            // CSE - PG
            new Course { Code = "CS701P", Title = "Advanced Algorithms",            Semester = 1, Department = "CSE" },
            new Course { Code = "CS705P", Title = "Advanced Database Systems",      Semester = 1, Department = "CSE" },
            new Course { Code = "CS712P", Title = "Distributed Systems",            Semester = 2, Department = "CSE" },
            new Course { Code = "CS720P", Title = "Cloud Computing",                Semester = 2, Department = "CSE" },
            new Course { Code = "CS731P", Title = "Deep Learning",                  Semester = 3, Department = "CSE" },
            new Course { Code = "CS740P", Title = "Information Retrieval",          Semester = 3, Department = "CSE" },

            // ECE - PG
            new Course { Code = "EC701P", Title = "Advanced Digital Signal Processing", Semester = 1, Department = "ECE" },
            new Course { Code = "EC708P", Title = "RF & Microwave Engineering",     Semester = 1, Department = "ECE" },
            new Course { Code = "EC715P", Title = "Advanced VLSI Design",           Semester = 2, Department = "ECE" },
            new Course { Code = "EC722P", Title = "FPGA Architectures & Design",    Semester = 2, Department = "ECE" },
            new Course { Code = "EC731P", Title = "MIMO & OFDM Systems",            Semester = 3, Department = "ECE" },
            new Course { Code = "EC742P", Title = "IoT Systems & Protocols",        Semester = 3, Department = "ECE" },

            // EEE - PG
            new Course { Code = "EE701P", Title = "Advanced Power System Analysis", Semester = 1, Department = "EEE" },
            new Course { Code = "EE706P", Title = "Smart Grids & Microgrids",       Semester = 1, Department = "EEE" },
            new Course { Code = "EE712P", Title = "Power System Protection",        Semester = 2, Department = "EEE" },
            new Course { Code = "EE720P", Title = "Power Electronics for Drives",   Semester = 2, Department = "EEE" },
            new Course { Code = "EE731P", Title = "High Voltage Engineering (PG)",  Semester = 3, Department = "EEE" },
            new Course { Code = "EE740P", Title = "Renewable Energy Integration",   Semester = 3, Department = "EEE" },
        };

        /// <summary>
        /// Returns a curated set of <b>MBA</b> courses across semesters 1–4.
        /// </summary>
        public static List<Course> GetMbaCourses() => new()
        {
            new Course { Code = "MB501",  Title = "Financial Accounting",            Semester = 1, Department = "MBA" },
            new Course { Code = "MB503",  Title = "Managerial Economics",            Semester = 1, Department = "MBA" },
            new Course { Code = "MB505",  Title = "Quantitative Techniques",         Semester = 1, Department = "MBA" },
            new Course { Code = "MB510",  Title = "Marketing Management",            Semester = 1, Department = "MBA" },

            new Course { Code = "MB521",  Title = "Financial Management",            Semester = 2, Department = "MBA" },
            new Course { Code = "MB523",  Title = "Human Resource Management",       Semester = 2, Department = "MBA" },
            new Course { Code = "MB525",  Title = "Operations Management",           Semester = 2, Department = "MBA" },
            new Course { Code = "MB528",  Title = "Business Analytics",              Semester = 2, Department = "MBA" },

            new Course { Code = "MB631",  Title = "Strategic Management",            Semester = 3, Department = "MBA" },
            new Course { Code = "MB633",  Title = "Entrepreneurship & Innovation",   Semester = 3, Department = "MBA" },
            new Course { Code = "MB636",  Title = "Consumer Behaviour",              Semester = 3, Department = "MBA" },
            new Course { Code = "MB639",  Title = "Project Management",              Semester = 3, Department = "MBA" },

            new Course { Code = "MB641",  Title = "International Business",          Semester = 4, Department = "MBA" },
            new Course { Code = "MB645",  Title = "Services Marketing",              Semester = 4, Department = "MBA" },
            new Course { Code = "MB648",  Title = "Investment Analysis & Portfolio", Semester = 4, Department = "MBA" },
            new Course { Code = "MB650",  Title = "Leadership & Change Management",  Semester = 4, Department = "MBA" }
        };

        /// <summary>
        /// Returns courses for the specified <paramref name="program"/>.
        /// </summary>
        public static List<Course> GetCourses(ProgramType program) =>
            program switch
            {
                ProgramType.UG => GetUgCourses(),
                ProgramType.PG => GetPgCourses(),
                ProgramType.MBA => GetMbaCourses(),
                _ => GetUgCourses()
            };

        /// <summary>
        /// Backward-compatible shortcut returning <b>UG</b> courses by default.
        /// </summary>
        public static List<Course> GetCourses() => GetCourses(ProgramType.UG);

        /// <summary>
        /// Computes a standard exam slot for the given <paramref name="date"/> and <paramref name="session"/>. FN → 09:30–12:30, AN → 14:00–17:00.
        /// </summary>
        /// <param name="date">The calendar date of the exam.</param>
        /// <param name="session">The session type (Forenoon or Afternoon).</param>
        /// <returns>A tuple of <c>(start, end)</c> times representing the exam duration.</returns>
        public static (DateTime start, DateTime end) Slot(DateTime date, ExamSession session)
        {
            var start = session == ExamSession.FN ? date.Date.AddHours(9).AddMinutes(30) : date.Date.AddHours(14);
            var end = start.AddHours(3);
            return (start, end);
        }

        /// <summary>
        /// Formats a time range as <c>HH:mm–HH:mm</c>.
        /// </summary>
        /// <param name="start">Start time.</param>
        /// <param name="end">End time.</param>
        /// <returns>A human-readable slot label.</returns>
        private static string FormatSlot(DateTime start, DateTime end) => $"{start:HH\\:mm}–{end:HH\\:mm}";

        /// <summary>
        /// Builds a complete list of exam appointments for the <b>UG</b> program (default).
        /// </summary>
        /// <returns>The generated set of <see cref="ExamAppointment"/> items.</returns>
        public static List<ExamAppointment> GetExamAppointments() => GetExamAppointments(ProgramType.UG);

        /// <summary>
        /// Builds a complete list of exam appointments for the specified <paramref name="program"/>,
        /// using semester-based pairing rules:
        /// <list type="bullet">
        /// <item><description>No exams on Sundays.</description></item>
        /// <item><description>UG/PG (Engineering): adjacent semester pairs per department (Option A). Example: (3,4), then (5,6), then repeat.</description></item>
        /// <item><description>PG (3 semesters): uses a fair cycle (1,2) → (3,1) → (2,3) to keep two semesters per workday.</description></item>
        /// <item><description>MBA: fixed pairs (1,3) then (2,4) on alternating workdays.</description></item>
        /// <item><description>4 slots/day (FN×2, AN×2) with hall rotation; avoid repeating invigilators within a day.</description></item>
        /// </list>
        /// </summary>
        /// <param name="program">The program for which to generate the schedule (UG, PG, MBA).</param>
        /// <returns>The generated set of <see cref="ExamAppointment"/> items.</returns>
        public static List<ExamAppointment> GetExamAppointments(ProgramType program)
        {
            var halls = GetHalls();
            var invigilators = GetInvigilators();
            var courses = GetCourses(program);

            var departments = courses
                .Select(course => course.Department)
                .Distinct()
                .OrderBy(department => department)
                .ToArray();

            var departmentSemesterQueues = new Dictionary<string, Dictionary<int, Queue<Course>>>(StringComparer.Ordinal);
            foreach (var department in departments)
            {
                var semesterQueueMap = new Dictionary<int, Queue<Course>>();
                var departmentCourses = courses.Where(course => course.Department == department);
                foreach (var semesterGroup in departmentCourses.GroupBy(course => course.Semester).OrderBy(group => group.Key))
                {
                    semesterQueueMap[semesterGroup.Key] = new Queue<Course>(semesterGroup);
                }
                departmentSemesterQueues[department] = semesterQueueMap;
            }

            var semesterPairSequenceByDepartment = new Dictionary<string, List<(int s1, int s2)>>(StringComparer.Ordinal);
            foreach (var department in departments)
            {
                var orderedSemesters = departmentSemesterQueues[department].Keys.OrderBy(sem => sem).ToList();
                semesterPairSequenceByDepartment[department] = BuildSemesterPairs(program, orderedSemesters);
            }

            var appointments = new List<ExamAppointment>();
            var baseDay = DateTime.Today.AddDays(3).Date;

            int invigilatorGlobalIndex = 0;
            var departmentStartIndex = 0;
            int workdayIndex = 0;

            var day = baseDay;
            while (HasAnyCoursesLeft(departmentSemesterQueues))
            {
                if (day.DayOfWeek == DayOfWeek.Sunday)
                {
                    day = day.AddDays(1);
                    continue;
                }

                var todaySemesterPairsByDepartment = new Dictionary<string, (int s1, int s2)>(StringComparer.Ordinal);
                foreach (var department in departments)
                {
                    var semesterPairSequence = semesterPairSequenceByDepartment[department];
                    if (semesterPairSequence.Count == 0)
                    {
                        todaySemesterPairsByDepartment[department] = (int.MinValue, int.MinValue);
                        continue;
                    }

                    var semesterPair = semesterPairSequence[workdayIndex % semesterPairSequence.Count];
                    todaySemesterPairsByDepartment[department] = semesterPair;
                }

                int hallRotationIndex = workdayIndex;
                var rotatedHalls = halls
                    .Skip(hallRotationIndex % halls.Count)
                    .Concat(halls.Take(hallRotationIndex % halls.Count))
                    .ToList();

                var dayPlan = new (ExamSession session, Hall hall)[]
                {
                    (ExamSession.FN, rotatedHalls[0]),
                    (ExamSession.FN, rotatedHalls[1]),
                    (ExamSession.AN, rotatedHalls[2]),
                    (ExamSession.AN, rotatedHalls[3])
                };

                var usedInvigilatorIdsToday = new HashSet<string>(StringComparer.Ordinal);

                foreach (var slot in dayPlan)
                {
                    if (!HasAnyCoursesLeft(departmentSemesterQueues))
                        break;

                    if (!TryPickDeptWithTodaySemCourses(
                            departments,
                            departmentSemesterQueues,
                            todaySemesterPairsByDepartment,
                            departmentStartIndex,
                            out var chosenDepartment))
                    {
                        break;
                    }

                    var (semester1, semester2) = todaySemesterPairsByDepartment[chosenDepartment];

                    if (!TryDequeueCourse(departmentSemesterQueues[chosenDepartment], semester1, semester2, out var courseToSchedule))
                    {
                        departmentStartIndex = (Array.IndexOf(departments, chosenDepartment) + 1) % departments.Length;
                        continue;
                    }

                    Invigilator selectedInvigilator = null!;
                    int invigilatorSelectionAttempts = 0;
                    while (invigilatorSelectionAttempts < invigilators.Count)
                    {
                        selectedInvigilator = invigilators[invigilatorGlobalIndex % invigilators.Count];
                        invigilatorGlobalIndex++;
                        if (!usedInvigilatorIdsToday.Contains(selectedInvigilator.Id))
                            break;
                        invigilatorSelectionAttempts++;
                    }
                    usedInvigilatorIdsToday.Add(selectedInvigilator.Id);

                    var (start, end) = Slot(day, slot.session);
                    var notes = $"{FormatSlot(start, end)}  •  Room: {slot.hall.Name}  •  Invigilator: {selectedInvigilator.Name}";

                    var appointment = new ExamAppointment
                    {
                        Start = start,
                        End = end,
                        Subject = $"{courseToSchedule.Code} – {courseToSchedule.Title}",
                        Notes = notes,
                        HallId = slot.hall.Id,
                        HallName = slot.hall.Name,
                        InvigilatorId = selectedInvigilator.Id,
                        InvigilatorName = selectedInvigilator.Name,
                        Background = slot.hall.Color
                    };

                    appointment.ResourceIds.Add(slot.hall.Id);
                    appointments.Add(appointment);

                    departmentStartIndex = (Array.IndexOf(departments, chosenDepartment) + 1) % departments.Length;
                }

                day = day.AddDays(1);
                workdayIndex++;
            }

            return appointments;
        }

        /// <summary>
        /// Builds the semester pair sequence according to program rules.
        /// UG/PG → adjacent pairs; if exactly 3 semesters, uses a fair cycle (1,2) → (3,1) → (2,3).
        /// MBA → fixed pairs (1,3) then (2,4).
        /// </summary>
        /// <param name="program">The program type (UG, PG, MBA).</param>
        /// <param name="semestersAscending">A sorted list of available semesters for a department.</param>
        /// <returns>A sequence of semester pairs in the order they should be used across workdays.</returns>
        private static List<(int s1, int s2)> BuildSemesterPairs(ProgramType program, List<int> semestersAscending)
        {
            var pairSequence = new List<(int, int)>();

            if (program == ProgramType.MBA)
            {
                pairSequence.Add((1, 3));
                pairSequence.Add((2, 4));
                return pairSequence;
            }

            // Engineering (UG/PG) – Option A: adjacent pairs
            if (semestersAscending.Count == 0)
            {
                return pairSequence;
            }

            if (semestersAscending.Count == 1)
            {
                // Only one semester present; pair it with itself
                var onlySemester = semestersAscending[0];
                pairSequence.Add((onlySemester, onlySemester));
                return pairSequence;
            }

            if (semestersAscending.Count == 3)
            {
                // Fair 2-per-day cycle for three semesters
                var semesterFirst = semestersAscending[0];
                var semesterSecond = semestersAscending[1];
                var semesterThird = semestersAscending[2];

                pairSequence.Add((semesterFirst, semesterSecond));
                pairSequence.Add((semesterThird, semesterFirst));
                pairSequence.Add((semesterSecond, semesterThird));
                return pairSequence;
            }

            for (int index = 0; index < semestersAscending.Count; index += 2)
            {
                var semester1 = semestersAscending[index];
                var semester2 = (index + 1 < semestersAscending.Count)
                    ? semestersAscending[index + 1]
                    : semestersAscending[index];
                pairSequence.Add((semester1, semester2));
            }

            return pairSequence;
        }

        /// <summary>
        /// Checks whether any department/semester queue still has courses left to schedule.
        /// </summary>
        /// <param name="departmentSemesterQueues">Map of department to semester queues.</param>
        /// <returns><c>true</c> if any queue has courses; otherwise <c>false</c>.</returns>
        private static bool HasAnyCoursesLeft(Dictionary<string, Dictionary<int, Queue<Course>>> departmentSemesterQueues)
        {
            foreach (var semesterQueueMap in departmentSemesterQueues.Values)
            {
                foreach (var courseQueue in semesterQueueMap.Values)
                {
                    if (courseQueue.Count > 0)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Finds the next department (round-robin) that has at least one course in today's allowed semester pair.
        /// </summary>
        /// <param name="departments">Ordered list of departments.</param>
        /// <param name="departmentSemesterQueues">Department to semester queues map.</param>
        /// <param name="todaySemesterPairsByDepartment">Today's allowed semester pairs per department.</param>
        /// <param name="startIndex">Round-robin starting index.</param>
        /// <param name="chosenDepartment">The department selected, if any.</param>
        /// <returns><c>true</c> if a department with available courses is found; otherwise <c>false</c>.</returns>
        private static bool TryPickDeptWithTodaySemCourses(
            string[] departments,
            Dictionary<string, Dictionary<int, Queue<Course>>> departmentSemesterQueues,
            Dictionary<string, (int s1, int s2)> todaySemesterPairsByDepartment,
            int startIndex,
            out string chosenDepartment)
        {
            for (int offset = 0; offset < departments.Length; offset++)
            {
                var department = departments[(startIndex + offset) % departments.Length];

                if (!todaySemesterPairsByDepartment.TryGetValue(department, out var allowedPair))
                    continue;

                if (HasCourseInSemesters(departmentSemesterQueues[department], allowedPair.s1, allowedPair.s2))
                {
                    chosenDepartment = department;
                    return true;
                }
            }

            chosenDepartment = "";
            return false;
        }

        /// <summary>
        /// Checks if the given semester queue map has any course in either of the two semesters.
        /// </summary>
        /// <param name="semesterQueueMap">Map of semester number to course queue.</param>
        /// <param name="semester1">First allowed semester.</param>
        /// <param name="semester2">Second allowed semester.</param>
        /// <returns><c>true</c> if any queue has at least one course; otherwise <c>false</c>.</returns>
        private static bool HasCourseInSemesters(Dictionary<int, Queue<Course>> semesterQueueMap, int semester1, int semester2)
        {
            return (semesterQueueMap.TryGetValue(semester1, out var queueForSemester1) && queueForSemester1.Count > 0)
                || (semesterQueueMap.TryGetValue(semester2, out var queueForSemester2) && queueForSemester2.Count > 0);
        }

        /// <summary>
        /// Tries to dequeue one course from the first allowed semester; if not found, tries the second.
        /// </summary>
        /// <param name="semesterQueueMap">Map of semester number to course queue.</param>
        /// <param name="semester1">First allowed semester.</param>
        /// <param name="semester2">Second allowed semester.</param>
        /// <param name="course">The dequeued course, if any.</param>
        /// <returns><c>true</c> if a course is dequeued; otherwise <c>false</c>.</returns>
        private static bool TryDequeueCourse(Dictionary<int, Queue<Course>> semesterQueueMap, int semester1, int semester2, out Course course)
        {
            if (semesterQueueMap.TryGetValue(semester1, out var queueForSemester1) && queueForSemester1.Count > 0)
            {
                course = queueForSemester1.Dequeue();
                return true;
            }

            if (semesterQueueMap.TryGetValue(semester2, out var queueForSemester2) && queueForSemester2.Count > 0)
            {
                course = queueForSemester2.Dequeue();
                return true;
            }

            course = null!;
            return false;
        }
    }
}