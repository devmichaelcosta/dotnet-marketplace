<?php
    namespace models;

    class CheckoutResult
    {
        public $success;
        public $orderId;

        public function __construct($success, $orderId)
        {
            $this->success = $success;
            $this->orderId = $orderId;
        }
    }