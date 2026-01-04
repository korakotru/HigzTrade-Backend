IxxxRepository 
	- Create / Update / Delete เท่านั้น
	- return เป็น Entity เท่านั้น (ถ้า return เป็น dto หรือค่าอื่นๆ ให้จัดเป็น ReadModel)


IxxxQuery (ReadModel)
	- Read-Side / View / Projection เท่านั้น
	- ไม่ return เป็น Entity  (ถ้า return เป็น entity จัดเป็น Repository)

