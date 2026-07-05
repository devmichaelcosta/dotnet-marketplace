<?php
    namespace controllers;
    use services\OrderService;

    class OrderDetailsController implements IBaseController
    {
        private $_orderService;

        public function __construct($factory)
        {
            $this->_orderService = new OrderService($factory);
        }
        
        public function proccessRequest() : void
        {
            $order = $this->_orderService->getOrderWithProducts();
            require "views/admin/order/details.php";
        }
    }
