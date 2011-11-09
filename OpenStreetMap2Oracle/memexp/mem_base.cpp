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