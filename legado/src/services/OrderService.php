<?php

namespace services;

use models\Order;
use models\OrderDetailsViewModel;

class OrderService
{
    private $_repoOrder;

    public function __construct($factory)
    {
        $this->_repoOrder = $factory->getOrderRepository();
    }

    public function getOrderWithProducts()
    {
        $orderId = intval($_GET["id"]);
        $order = $this->_repoOrder->getOrderWithProducts($orderId);

        return $order;
    }
}
