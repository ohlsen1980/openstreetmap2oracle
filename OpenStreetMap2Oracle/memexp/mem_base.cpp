/* 
  memexp - A x64 windows console application for extending the capabilities  
                 of .NET CLR apps suffering from 2GB memory limit
  -------------------------------------------------------------------------------
  Copyright (C) 2011  Christian Möller
  -------------------------------------------------------------------------------
  This program is free software; you can redistribute it and/or
  modify it under the terms of the GNU General Public License
  as published by the Free Software Foundation; either version 2
  of the License, or (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program; if not, write to the Free Software
  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

#include "StdAfx.h"
#include "mem_base.h"

mem_base::mem_base(void)
{
	
}

mem_base::~mem_base(void)
{
	free(&_data);
}

void mem_base::mem_base_set(long _id, char *point) {
	_data[_id] = point;
}

char *mem_base::mem_base_get(long _id) {
	if (_data.count(_id) > 0) {
		_iterator = _data.find(_id);
		return _iterator->second;
	} else {
		return "";
	}

}

void mem_base::mem_base_clear() {
	_data.clear();
}

t_cachemap_iter mem_base::first() {
	return _data.begin();
}

t_cachemap_iter mem_base::end() {
	return _data.end();
}