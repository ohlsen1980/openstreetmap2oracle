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

#pragma once

#include <map>
#include <string>

using namespace std;

class mem_base
{
	
private:
	map<long, char*> _data;
	map<long, char*>::iterator _iterator;

public:
	mem_base(void);
	~mem_base(void);
	void mem_base_set(long _id, char* data);
	char* mem_base_get(long _id);
};

