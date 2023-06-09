﻿using MySqlConnector;

namespace Underground.ORM.Core.Translator.Syntax.Token.Declaration
{
    public class StringToken : MySqlSyntaxToken
    {
        public bool IsDeclaration { get => Is.Declaration; set => Is.Declaration = value; }

        public bool IsString { get => Is.String; set => Is.String = value; }

        public bool IsDbType { get => Is.DbType; set => Is.DbType = value; }

        public StringToken(string token) :
            base(token)
        {
            IsString = true;
            IsDbType = true;
            IsDeclaration = true;
            DbType = System.Data.DbType.String;

            //Token = FormatString(token);
        }

        private string FormatString(string token)
        {
            return MySqlHelper.EscapeString(token);
        }
    }
}
