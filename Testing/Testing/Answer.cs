//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Testing
{
    using System;
    using System.Collections.Generic;
    
    public partial class Answer
    {
        public int id { get; set; }
        public int questionId { get; set; }
        public string answer_name { get; set; }
        public int correctly { get; set; }
    
        public virtual Question Question { get; set; }
    }
}
