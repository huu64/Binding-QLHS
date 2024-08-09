namespace WindowsFormsApplication1
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Employee")]
    public partial class Employee
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EmployeeID { get; set; }

        [StringLength(20)]
        public string LastName { get; set; }

        [StringLength(30)]
        public string FirstName { get; set; }

        public int? DepartmentID { get; set; }

        public bool? Sex { get; set; }

        public virtual Department Department { get; set; }
    }
}
