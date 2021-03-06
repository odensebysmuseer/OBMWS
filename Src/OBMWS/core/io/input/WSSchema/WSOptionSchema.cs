﻿#region license
//	GNU General Public License (GNU GPLv3)
 
//	Copyright © 2016 Odense Bys Museer

//	Author: Andriy Volkov

//	Source URL:	https://github.com/odensebysmuseer/OBMWS

//	This program is free software: you can redistribute it and/or modify
//	it under the terms of the GNU General Public License as published by
//	the Free Software Foundation, either version 3 of the License, or
//	(at your option) any later version.

//	This program is distributed in the hope that it will be useful,
//	but WITHOUT ANY WARRANTY; without even the implied warranty of
//	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
//	See the GNU General Public License for more details.

//	You should have received a copy of the GNU General Public License
//	along with this program.  If not, see <http://www.gnu.org/licenses/>.
#endregion

namespace OBMWS
{
    public class WSOptionSchema : WSBaseSchema
    {
        public WSJArray option { get; private set; }
        public WSOptionSchema(WSJArray _json) { option = _json; }

        internal WSFilter GetFilter()
        {
            WSCombineFilter filter = new WSCombineFilter(WSCombineFilter.SQLMode.OrElse);
            foreach (WSJson jOption in option.Value)
            {
                if (jOption is WSJValue)
                {
                    WSJValue jval = (WSJValue)jOption;
                    if (jval.Value.IsTrue() || jval.Value.IsFalse()) { filter.Add(new WSBoolOFilter(jval)); }
                }
            }
            return filter != null && filter.IsValid ? filter.Count==1? filter[0] :filter : null;
        }
    }
}