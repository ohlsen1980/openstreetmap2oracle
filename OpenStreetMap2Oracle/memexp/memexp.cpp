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

#include "stdafx.h"

#define COMMAND_PUT		"PUT"
#define COMMAND_PUTA	"PUTA"
#define COMMAND_COMMIT	"COMMIT"
#define COMMAND_GET		"GET"
#define COMMAND_EXIT	"EXIT"
#define COMMAND_COUNT	"COUNT"
#define COMMAND_CLEAR	"CLEAR"
#define COMMAND_LIST	"LIST"

using namespace std;

long _cin_PUT(char* buffer_id, char* buffer_data) {
	long _id;

	cin >> buffer_id;

	if (strcmp(buffer_id, COMMAND_COMMIT) == 0) {
		return 0;
	}

	cin >> buffer_data;

	_id = atol(buffer_id);
	return _id;
}

int _tmain(int argc, _TCHAR* argv[])
{
	mem_base *mbase = new mem_base();

	char *buffer_id = (char*) malloc(sizeof(char) * 64);
	char *buffer_data = (char*) malloc(sizeof(char) * 128);
	char *buffer_cmd = (char*) malloc(sizeof(char) * 10);
	long _id;

	while (true) {
		cin >> buffer_cmd;
		
		if (buffer_cmd != NULL) {
			if (strcmp(buffer_cmd, COMMAND_PUT) == 0) {
				_id = _cin_PUT(buffer_id, buffer_data);
				mbase->mem_base_set(_id, buffer_data);
			} else if (strcmp(buffer_cmd, COMMAND_PUTA) == 0) {
				while ((_id = _cin_PUT(buffer_id, buffer_data)) > 0) {
					mbase->mem_base_set(_id, buffer_data);
				} 
			} else if (strcmp(buffer_cmd, COMMAND_GET) == 0) {
				cin >> buffer_id;
				_id = atol(buffer_id);
				cout << mbase->mem_base_get(_id) << endl;
			} else if (strcmp(buffer_cmd, COMMAND_EXIT) == 0) {
				exit(0);
			} else if (strcmp(buffer_cmd, COMMAND_COUNT) == 0) {
				
			} else if (strcmp(buffer_cmd, COMMAND_CLEAR) == 0) {
				mbase->mem_base_clear();
			} else if (strcmp(buffer_cmd, COMMAND_LIST) == 0) {
				t_cachemap_iter iterator;
				t_cachemap_iter end = mbase->end();
				for (iterator = mbase->first(); iterator != end; ++iterator) {
					cout << iterator->first << " : " << iterator->second << endl;
				}
			}
		} else {
			exit(0);
		}
	}
}

