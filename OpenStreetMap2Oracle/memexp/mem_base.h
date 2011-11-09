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

