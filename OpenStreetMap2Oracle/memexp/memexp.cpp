// memexp.cpp : Definiert den Einstiegspunkt für die Konsolenanwendung.
//

#include "stdafx.h"
#include "mem_base.h"
#include <cstdio>
#include <iostream>

using namespace std;

int _tmain(int argc, _TCHAR* argv[])
{
	mem_base *mbase = new mem_base();

	char *buffer_id = (char*) malloc(sizeof(char) * 64);
	char *buffer_data = (char*) malloc(sizeof(char) * 128);
	char *buffer_cmd = (char*) malloc(sizeof(char) * 10);
	float _id;

	while (true) {
		cin >> buffer_cmd;
		
		if (strcmp(buffer_cmd, "PUT") == 0) {
			cin >> buffer_id;
			cin >> buffer_data;
			_id = atol(buffer_id);
			mbase->mem_base_set(_id, buffer_data);
		} else if (strcmp(buffer_cmd, "GET") == 0) {
			cin >> buffer_id;
			_id = atol(buffer_id);
			cout << mbase->mem_base_get(_id) << endl;
		} else if (strcmp(buffer_cmd, "EXIT") == 0) {
			exit(0);
		}
	}
}

