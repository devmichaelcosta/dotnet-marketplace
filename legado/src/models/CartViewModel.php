<?php
    namespace models;

    class CartViewModel
    {
        private $total;
        private $products;
        private $cartGroup;

        public function __construct($cartGroup,$products, $total)
        {
            $this->cartGroup = $cartGroup;
            $this->total = $total;
            $this->products = $products;
        }

        public function getTotal()
        {
            return $this->total;
        }

        public function getProductTotalValue($productId)
        {
            $product = $this->getProduct($productId);
            return $product->calcValue();
        }

        public function getProductTotalValueFormatted($productId)
        {
            
            return "R$ ".  number_format($this->getProductTotalValue($productId),2,",",".");
        }
        public function getProduct($productId)
        {
            $product = null;
            if (!is_null($this->getProducts()))
            {
                foreach($this->getProducts() as $productItem)
                {
                    if ($productItem->getProductId() === $productId){
                        $product= $productItem;
                        break;
                    }
                }
            }
            return $product;
        }

        public function updateQtdProduct($productId,$qtd)
        {
            $product = $this->getProduct($productId);
            if (!is_null($product))
                $product->setQtd($qtd);
        }

        public function getSubTotal()
        {

            $valor = 0;
            if (!is_null($this->getProducts())){
                foreach($this->getProducts() as $productItem){
                    $valor += $productItem->getQtd() * $productItem->getPrice();
                }
            }
            return $valor;
        }

        public function getSubTotalFormatted(){
            return "R$ ".  number_format($this->getSubTotal(),2,",",".");
        }

        public function getSubTotalComCondicoes()
        {
            $valor = 0;
            if (!is_null($this->getProducts()))
            {
                foreach($this->getProducts() as $productItem)
                {
                    $valor += $productItem->getQtd() * $productItem->getPrice();
                }
            }
            return $valor;
        }

        public function getFreteValor()
        {
            $valor = 0;
            return $valor;
        }

        public function getQtdFromProduct($productId)
        {
            $qtd = 0;
            if (!is_null($this->getProducts())){
                foreach($this->getProducts() as $productItem){
                    if ($productItem->getProductId() == $productId)
                        $qtd = $productItem->getQtd();
                }
            }
            return $qtd;
        }

        public function getTotalProdutos()
        {
            $qtdItensCarrinho = 0;
            if (!is_null($this->getProducts())){
                foreach($this->getProducts() as $productItem){
                    $qtdItensCarrinho += $productItem->getQtd();
                }
            }
            return $qtdItensCarrinho;
        }

        public function getTotalFinal()
        {
            return $this->getFreteValor() + $this->getSubTotalComCondicoes();
        }

        public function getTotalFormatted()
        {
            return "R$ ".  number_format($this->getTotalFinal(),2,",",".");
        }
        /**
         * Get the value of products
         */ 
        public function getProducts()
        {
            return $this->products;
        }

        public function getCartGroup()
        {
            return $this->cartGroup;
        }

        public function removeProduct($productId)
        {
            //echo count($this->getProducts()) . "<br/>";

            if (!is_null($this->getProducts()))
            {
                $posicao = -1;
                $indexAtual = 0; 
                foreach ($this->getProducts() as $productItem)
                {
                    if ($productId === $productItem->getProductId())
                    {
                        $posicao = $indexAtual;
                    }
                    $indexAtual++;
                }

                if ($posicao > -1)
                {
                    $itens = $this->getProducts();
                    array_splice($itens, $posicao, 1);
                    $this->products = $itens;
                }
            }
        }
        
        public function addProduct($product)
        {
            if (!is_null($this->getProducts()))
            {
                //ja tem produtos...
                $existe = false;
                foreach ($this->getProducts() as $productItem)
                {
                    if ($product->getProductId() == $productItem->getProductId())
                    {
                        $productItem->incrementQtd(); 
                        $existe = true;
                    } 
                }

                if (!$existe)
                    $this->products[] = $product;
            }
        }

        public function getQtdProducts()
        {
            $total = 0;
            foreach ($this->getProducts() as $productItem)
            {
                $total += $productItem->getQtd();
                
            }
            return $total;
        }
    }