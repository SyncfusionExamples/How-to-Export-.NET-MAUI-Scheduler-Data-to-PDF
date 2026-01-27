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
    /// Provides deterministic data (halls, invigilators, courses) and utility methods
    /// for building complete exam schedules used by the app. Includes program-aware
    /// scheduling for UG, PG, and MBA with constraints:
    /// - No Sunday exams,
    /// - Engineering (UG/PG): adjacent semester pairs per department (Option A),
    /// - MBA: fixed pairs (1,3) then (2,4) on alternating workdays,
    /// - 4 slots/day (FN×2, AN×2), hall rotation, and minimal invigilator repeats per day.
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
        /// Computes a standard exam slot for the given <paramref name="date"/> and <paramref name="session"/>.
        /// FN → 09:30–12:30, AN → 14:00–17:00.
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
        /// <param name="s">Start time.</param>
        /// <param name="e">End time.</param>
        /// <returns>A human-readable slot label.</returns>
        private static string FormatSlot(DateTime s, DateTime e) => $"{s:HH\\:mm}–{e:HH\\:mm}";

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

            // Build department -> semester -> queue of courses
            var departments = courses.Select(c => c.Department).Distinct().OrderBy(d => d).ToArray();
            var deptSemQueues = new Dictionary<string, Dictionary<int, Queue<Course>>>(StringComparer.Ordinal);
            foreach (var dept in departments)
            {
                var semMap = new Dictionary<int, Queue<Course>>();
                foreach (var grp in courses.Where(c => c.Department == dept).GroupBy(c => c.Semester).OrderBy(g => g.Key))
                {
                    semMap[grp.Key] = new Queue<Course>(grp);
                }
                deptSemQueues[dept] = semMap;
            }

            // Build semester pair sequence per department according to rules
            var pairSeqPerDept = new Dictionary<string, List<(int s1, int s2)>>(StringComparer.Ordinal);
            foreach (var dept in departments)
            {
                var sems = deptSemQueues[dept].Keys.OrderBy(s => s).ToList();
                pairSeqPerDept[dept] = BuildSemesterPairs(program, sems);
            }

            var appointments = new List<ExamAppointment>();
            var baseDay = DateTime.Today.AddDays(3).Date;

            int invIndexGlobal = 0;                 // rotate globally across days
            var deptStartIndex = 0;                  // rotate department start per slot
            int workDayIndex = 0;                    // counts non-Sunday days only

            var day = baseDay;
            while (HasAnyCoursesLeft(deptSemQueues))
            {
                // Skip Sundays without advancing the semester pair pattern
                if (day.DayOfWeek == DayOfWeek.Sunday)
                {
                    day = day.AddDays(1);
                    continue;
                }

                // Determine today's semester pair for each department
                var todayPairs = new Dictionary<string, (int s1, int s2)>(StringComparer.Ordinal);
                foreach (var dept in departments)
                {
                    var seq = pairSeqPerDept[dept];
                    if (seq.Count == 0)
                    {
                        todayPairs[dept] = (int.MinValue, int.MinValue);
                        continue;
                    }
                    var pair = seq[workDayIndex % seq.Count];
                    todayPairs[dept] = pair;
                }

                // Rotate halls per day: FN => [0,1], AN => [2,3]
                int dayIndex = workDayIndex; // tie hall rotation to workdays
                var hallsRotated = halls
                    .Skip(dayIndex % halls.Count)
                    .Concat(halls.Take(dayIndex % halls.Count))
                    .ToList();

                var dayPlan = new (ExamSession session, Hall hall)[]
                {
                    (ExamSession.FN, hallsRotated[0]),
                    (ExamSession.FN, hallsRotated[1]),
                    (ExamSession.AN, hallsRotated[2]),
                    (ExamSession.AN, hallsRotated[3])
                };

                var usedInvigilatorsToday = new HashSet<string>(StringComparer.Ordinal);

                // Try to fill today's 4 slots
                foreach (var slot in dayPlan)
                {
                    if (!HasAnyCoursesLeft(deptSemQueues))
                        break;

                    // Find next department (round-robin) that still has any course in today's allowed semesters
                    if (!TryPickDeptWithTodaySemCourses(departments, deptSemQueues, todayPairs, deptStartIndex, out var chosenDept))
                    {
                        // No department can supply a course for today's pairs; stop filling further slots
                        break;
                    }

                    var (s1, s2) = todayPairs[chosenDept];

                    // Pick a course from allowed semesters (prefer lower semester first)
                    if (!TryDequeueCourse(deptSemQueues[chosenDept], s1, s2, out var course))
                    {
                        // Shouldn't happen due to the check above, but guard anyway.
                        deptStartIndex = (Array.IndexOf(departments, chosenDept) + 1) % departments.Length;
                        continue;
                    }

                    // Choose invigilator (avoid repeating within the same day if possible)
                    Invigilator inv = null!;
                    int tries = 0;
                    while (tries < invigilators.Count)
                    {
                        inv = invigilators[invIndexGlobal % invigilators.Count];
                        invIndexGlobal++;
                        if (!usedInvigilatorsToday.Contains(inv.Id))
                            break;
                        tries++;
                    }
                    usedInvigilatorsToday.Add(inv.Id);

                    var (start, end) = Slot(day, slot.session);
                    var notes = $"{FormatSlot(start, end)}  •  Room: {slot.hall.Name}  •  Invigilator: {inv.Name}";

                    var appointment = new ExamAppointment
                    {
                        Start = start,
                        End = end,
                        Subject = $"{course.Code} – {course.Title}",
                        Notes = notes,
                        HallId = slot.hall.Id,
                        HallName = slot.hall.Name,
                        InvigilatorId = inv.Id,
                        InvigilatorName = inv.Name,
                        Background = slot.hall.Color
                    };

                    appointment.ResourceIds.Add(slot.hall.Id);
                    appointments.Add(appointment);

                    // Rotate department start for next slot
                    deptStartIndex = (Array.IndexOf(departments, chosenDept) + 1) % departments.Length;
                }

                // Advance to next day (workday index only)
                day = day.AddDays(1);
                workDayIndex++;
            }

            return appointments;
        }

        /// <summary>
        /// Builds the semester pair sequence according to program rules.
        /// UG/PG → adjacent pairs; if exactly 3 semesters, use a fair cycle (1,2)->(3,1)->(2,3).
        /// MBA → fixed (1,3)->(2,4) pairs.
        /// </summary>
        private static List<(int s1, int s2)> BuildSemesterPairs(ProgramType program, List<int> semestersAsc)
        {
            var result = new List<(int, int)>();
            if (program == ProgramType.MBA)
            {
                result.Add((1, 3));
                result.Add((2, 4));
                return result;
            }

            // Engineering (UG/PG) – Option A: adjacent pairs
            if (semestersAsc.Count == 0)
                return result;

            if (semestersAsc.Count == 1)
            {
                // Only one semester present; pair it with itself
                result.Add((semestersAsc[0], semestersAsc[0]));
                return result;
            }

            if (semestersAsc.Count == 3)
            {
                // Fair 2-per-day cycle for three semesters
                var a = semestersAsc[0];
                var b = semestersAsc[1];
                var c = semestersAsc[2];
                result.Add((a, b));
                result.Add((c, a));
                result.Add((b, c));
                return result;
            }

            // Even or >=4: pair adjacent (1,2), (3,4), (5,6)...
            for (int i = 0; i < semestersAsc.Count; i += 2)
            {
                var s1 = semestersAsc[i];
                var s2 = (i + 1 < semestersAsc.Count) ? semestersAsc[i + 1] : semestersAsc[i]; // guard odd
                result.Add((s1, s2));
            }
            return result;
        }

        /// <summary>
        /// Checks whether any department/semester queue still has courses left.
        /// </summary>
        private static bool HasAnyCoursesLeft(Dictionary<string, Dictionary<int, Queue<Course>>> deptSemQueues)
        {
            foreach (var semMap in deptSemQueues.Values)
            {
                foreach (var q in semMap.Values)
                    if (q.Count > 0) return true;
            }
            return false;
        }

        /// <summary>
        /// Finds the next department (round-robin) that has at least one course in today's allowed semesters.
        /// </summary>
        private static bool TryPickDeptWithTodaySemCourses(
            string[] departments,
            Dictionary<string, Dictionary<int, Queue<Course>>> deptSemQueues,
            Dictionary<string, (int s1, int s2)> todayPairs,
            int startIndex,
            out string chosenDept)
        {
            for (int i = 0; i < departments.Length; i++)
            {
                var dept = departments[(startIndex + i) % departments.Length];
                if (!todayPairs.TryGetValue(dept, out var pair))
                    continue;

                if (HasCourseInSemesters(deptSemQueues[dept], pair.s1, pair.s2))
                {
                    chosenDept = dept;
                    return true;
                }
            }
            chosenDept = "";
            return false;
        }

        /// <summary>
        /// Checks if the semester map has any course in either s1 or s2.
        /// </summary>
        private static bool HasCourseInSemesters(Dictionary<int, Queue<Course>> semMap, int s1, int s2)
        {
            return (semMap.TryGetValue(s1, out var q1) && q1.Count > 0)
                || (semMap.TryGetValue(s2, out var q2) && q2.Count > 0);
        }

        /// <summary>
        /// Tries to dequeue one course from s1 first, then s2.
        /// </summary>
        private static bool TryDequeueCourse(Dictionary<int, Queue<Course>> semMap, int s1, int s2, out Course course)
        {
            if (semMap.TryGetValue(s1, out var q1) && q1.Count > 0)
            {
                course = q1.Dequeue();
                return true;
            }
            if (semMap.TryGetValue(s2, out var q2) && q2.Count > 0)
            {
                course = q2.Dequeue();
                return true;
            }
            course = null!;
            return false;
        }
    }
}