package controller

import (
	"time"
)

const (
	simplePlanReconcileInterval time.Duration = time.Second * 10
	accountReconcileInterval    time.Duration = time.Second * 10
	timeoutGetRequest           time.Duration = time.Second * 10
)
