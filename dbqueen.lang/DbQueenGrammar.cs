using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;

namespace dbqueen.lang
{
    [Language("dQ", "0.1", "DbQueen Grammar")]
    public class DbQueenGrammar : Grammar 
    {
        public DbQueenGrammar() 
            : base(true)
        {
            #region Terminals
            CommentTerminal blockComment = new CommentTerminal("block-comment", "/*", "*/");
            CommentTerminal lineComment = new CommentTerminal("line-comment", "//", "\r", "\n", "\u2085", "\u2028", "\u2029");
            NonGrammarTerminals.Add(blockComment);
            NonGrammarTerminals.Add(lineComment);

            NumberLiteral number = new NumberLiteral("number");
            IdentifierTerminal identifier = new IdentifierTerminal("identifier");
            StringLiteral string_lit = new StringLiteral("string", "'", StringOptions.AllowsDoubledQuote);
            #endregion

            #region NonTerminals
            NonTerminal module = new NonTerminal("module");
            NonTerminal schema = new NonTerminal("schema");
            NonTerminal schemas = new NonTerminal("schemas");
            NonTerminal schema_stmt = new NonTerminal("schema_stmt");
            NonTerminal schema_stmts = new NonTerminal("schema_stmts");
            NonTerminal tbl_decl = new NonTerminal("tbl_decl");
            NonTerminal simple_decl = new NonTerminal("simple_decl");
            NonTerminal semi_decl = new NonTerminal("semi_decl");
            NonTerminal id_list = new NonTerminal("id_list");
            NonTerminal simple_id = new NonTerminal("id");
            NonTerminal id = new NonTerminal("id");
            NonTerminal module_id = new NonTerminal("module_id");
            NonTerminal module_id_single = new NonTerminal("module_id_single");
            NonTerminal module_id_mask = new NonTerminal("module_id_mask");
            NonTerminal module_id_list = new NonTerminal("module_id_list");
            NonTerminal field_decls = new NonTerminal("field_decls");
            NonTerminal field_decl = new NonTerminal("field_decl");
            NonTerminal field_tbl_decl = new NonTerminal("field_tbl_decl");
            NonTerminal variable_tbl_def = new NonTerminal("variable_tbl_def");
            NonTerminal simple_tbl_def = new NonTerminal("simple_tbl_def");
            NonTerminal dec_tbl_def = new NonTerminal("dec_tbl_def");
            NonTerminal complex_tbl_def = new NonTerminal("complex_tbl_def");
            NonTerminal tbl_inline_type_def = new NonTerminal("tbl_inline_type_def");
            NonTerminal vector_tbl_def = new NonTerminal("vector_tbl_def");
            NonTerminal scalar_tbl_def = new NonTerminal("scalar_tbl_def");
            NonTerminal int_tbl_def = new NonTerminal("int_tbl_def");
            NonTerminal string_size_def = new NonTerminal("string_size_def");
            NonTerminal string_tbl_def = new NonTerminal("string_tbl_def");
            NonTerminal lookup_tbl_def = new NonTerminal("lookup_tbl_def");
            NonTerminal extends_opt = new NonTerminal("extends_opt");

            NonTerminal fld_opt = new NonTerminal("fld_opt");
            NonTerminal fld_opts = new NonTerminal("fld_opts");
            NonTerminal default_fld_opt = new NonTerminal("default_fld_opt");
            NonTerminal nullable_fld_opt = new NonTerminal("nullable_fld_opt");

            NonTerminal precision_def = new NonTerminal("precision_def");
            NonTerminal scale_def = new NonTerminal("scale_def");

            NonTerminal seed_def = new NonTerminal("seed_def");
            NonTerminal increment_def = new NonTerminal("increment_def");
            NonTerminal autoinc_tbl_def = new NonTerminal("autoinc_tbl_def");

            NonTerminal join_decl = new NonTerminal("join_decl");

            NonTerminal setter_stmt = new NonTerminal("on_event_stmt");
            NonTerminal expression = new NonTerminal("expressioin");

            #endregion

            #region Rules

            #region Ids
            simple_id.Rule = identifier;
            id.Rule = MakePlusRule(id, ToTerm("."), simple_id);
            id_list.Rule = MakePlusRule(id_list, ToTerm(","), id);
            #endregion

            #region Module Ids
            module_id_single.Rule = id;
            module_id_mask.Rule = id + "." + "*";
            module_id.Rule = module_id_single | module_id_mask;
            module_id_list.Rule = MakePlusRule(module_id_list, ToTerm(","), module_id);
            #endregion

            #region Basic statements
            schema_stmt.Rule = simple_decl | tbl_decl;
            schema_stmts.Rule = MakeStarRule(schema_stmts, schema_stmt);
            simple_decl.Rule = semi_decl + ";";
            semi_decl.Rule = join_decl;
            #endregion

            #region Schema
            schema.Rule = ToTerm("schema") + simple_id + "{" + schema_stmts + "}";
            schemas.Rule = MakeStarRule(schemas, schema);
            #endregion

            join_decl.Rule = ToTerm("join") + id +
                (ToTerm("->") | ToTerm("<-") | ToTerm("*->") | ToTerm("<-*") | ToTerm("*-*")) + id;


            #region Table/struct
            field_decls.Rule = MakePlusRule(field_decls, ToTerm(","), field_decl);
            field_decl.Rule =
                (simple_id + id) |
                (simple_id + vector_tbl_def) |
                (simple_id + lookup_tbl_def) |
                (simple_id + nullable_fld_opt + scalar_tbl_def + fld_opts);

            variable_tbl_def.Rule = simple_tbl_def | dec_tbl_def | int_tbl_def | string_tbl_def;
            scalar_tbl_def.Rule = variable_tbl_def | autoinc_tbl_def | tbl_inline_type_def | id;
            vector_tbl_def.Rule = ToTerm("*") + scalar_tbl_def;
            lookup_tbl_def.Rule = (ToTerm("->") | ToTerm("?->") | ToTerm("*->")) + id;

            fld_opts.Rule = MakeStarRule(fld_opts, fld_opt);
            fld_opt.Rule = default_fld_opt;
            nullable_fld_opt.Rule = Empty | ToTerm("?");
            default_fld_opt.Rule = Empty | (ToTerm("default") + "(" + expression + ")");

            simple_tbl_def.Rule = 
                ToTerm("money") | "bool" | "date" | "datetime" | "time" |
                "float" | "uid" 
                ;
            
            int_tbl_def.Rule =
                ToTerm("int8") | "int16" | "int32" | "int64" |
                "tinyint" | "smallint" | "int" | "bigint";

            precision_def.Rule = number;
            scale_def.Rule = number;
            dec_tbl_def.Rule =
                ToTerm("decimal") |
                (ToTerm("decimal") + "(" + precision_def + ")") |
                (ToTerm("decimal") + "(" + precision_def + "," + scale_def + ")");


            seed_def.Rule = number;
            increment_def.Rule = number;
            autoinc_tbl_def.Rule = 
                ToTerm("autoinc") |
                (ToTerm("autoinc") + "(" + int_tbl_def + ")") |
                (ToTerm("autoinc") + "(" + int_tbl_def + "," + seed_def + "," + increment_def + ")") |
                (ToTerm("autoinc") + "(" + int_tbl_def + "," + seed_def + ")") |
                (ToTerm("autoinc") + "(" + seed_def + "," + increment_def + ")") |
                (ToTerm("autoinc") + "(" + seed_def + ")");

            string_size_def.Rule = number | "max";

            string_tbl_def.Rule =
                ToTerm("string") |
                (ToTerm("string") + "(" + string_size_def + ")");

            extends_opt.Rule = Empty | (ToTerm("extends") + id);

            tbl_decl.Rule =
                (ToTerm("table") + simple_id + extends_opt + "{" + field_decls + "}") |
                (ToTerm("table") + "from" + id + ";");

            tbl_inline_type_def.Rule = ToTerm("{") + field_decls + "}";
            #endregion

            setter_stmt.Rule = simple_id + "=" + expression;

            expression.Rule = number | identifier | string_lit;

            module.Rule =
                (ToTerm("module") + simple_id + "{" + schemas + "}") |
                schemas;
            #endregion

            this.Root = module;
        }
    }
}
