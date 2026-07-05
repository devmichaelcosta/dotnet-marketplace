<?php

namespace services;

use models\responses\UpdateQtdProductResponse;
use models\CartViewModel;
use models\ProductCart;
use models\Order;
use models\Checkout;
use models\CheckoutResult;
use models\OrderItem;
use models\Address;
use Exception;

class CartService
{
    private $_repoCart;
    private $_repoProd;
    private $_repoStates;
    private $_repoOrder;
    private $_repoAddress;

    public function __construct($factory)
    {
        $this->_repoCart = $factory->getCartRepository();
        $this->_repoProd = $factory->getProductRepository();
        $this->_repoStates = $factory->getStateRepository();
        $this->_repoOrder = $factory->getOrderRepository();
        $this->_repoAddress = $factory->getAddressRepository();
    }

    public function updateQtdProduct($productId, $qtd)
    {
        $cartViewModel = null;
        if (isset($_SESSION["cart"])) {
            $cartViewModel = $_SESSION["cart"];

            //verificando estoque...
            $stock = $this->_repoProd->getCurrentStock($productId);
            if ($stock >= $qtd) {
                //tem estoque uhul !!!
                $cartViewModel->updateQtdProduct($productId, $qtd);
                $cartViewModel = $_SESSION["cart"];
                return new UpdateQtdProductResponse(
                    true,
                    "Estoque atualizado com sucesso.",
                    $stock - $qtd,
                    $cartViewModel->getQtdFromProduct($productId),
                    $cartViewModel->getProductTotalValueFormatted($productId),
                    $cartViewModel->getSubTotalFormatted()
                );
            } else {
                //xi deu ruim, não tem estoque...
                return new UpdateQtdProductResponse(
                    false,
                    "Ops desculpe, o estoque atual é de " . $stock . ", e não é suificiente para a quantidade solicitada.",
                    $stock,
                    $cartViewModel->getQtdFromProduct($productId),
                    $cartViewModel->getProductTotalValueFormatted($productId),
                    $cartViewModel->getSubTotalFormatted()
                );
            }
        }
    }

    public function removeProduct($productId)
    {
        //echo "RemoveFromCartController productId: " . $productId ;
        $cartViewModel = null;
        if (isset($_SESSION["cart"])) {
            $cartViewModel = $_SESSION["cart"];
            $cartViewModel->removeProduct($productId);
            $cartViewModel = $_SESSION["cart"];
        }
        return $cartViewModel;
    }

    public function addProduct($productId)
    {
        //obtendo produto selecionado
        $product = $this->_repoProd->getById($productId);
        if (isset($_SESSION["cart"])) {
            //ja existe um carrinho na sessao...
            $cartViewModel = $_SESSION["cart"];

            $productCart = $cartViewModel->getProduct($productId);
            $stockAvailble =  $product->getStock();
            if ($productCart !== null)
                $stockAvailble =  $product->getStock() - $productCart->getQtd();

            //se tiver estoque adicionamos no carrinho...
            if ($stockAvailble > 0) {
                //obtendo imagem principal...
                $firstImage = null;
                if (!is_null($product->getImages()) && count($product->getImages()) > 0)
                    $firstImage = $product->getImages()[0]["FileName"];

                //adicionando produto no objeto de carrinho
                $cartViewModel->addProduct(
                    new ProductCart(
                        null,
                        $cartViewModel->getCartGroup(),
                        $product->getId(),
                        $product->getTitle(),
                        $product->getPrice(),
                        1,
                        $firstImage,
                        $product->getPrice()
                    )
                );
            }

            //atualizando na sessao
            $_SESSION["cart"] = $cartViewModel;
            return $cartViewModel;
        } else {
            //carrinho nao existe, criando
            $cartGroup = md5(uniqid(rand(), true));
            $products = array();
            $firstImage = null;
            if (!is_null($product->getImages()) && count($product->getImages()) > 0)
                $firstImage = $product->getImages()[0]["FileName"];

            $products[] = new ProductCart(
                null,
                $cartGroup,
                $product->getId(),
                $product->getTitle(),
                $product->getPrice(),
                1,
                $firstImage,
                $product->getPrice()
            );
            $cartViewModel = new CartViewModel(
                $cartGroup,
                $products,
                $product->getPrice()
            );
            $_SESSION["cart"] = $cartViewModel;
            return $cartViewModel;
        }
    }

    public function getCurrentCart()
    {
        if (isset($_SESSION["cart"])) {
            //ja existe um carrinho
            return $_SESSION["cart"];
        } else {
            return null;
        }
    }

    public function getCheckoutViewModel()
    {
        $carrinho = $_SESSION["cart"];
        $states = $this->_repoStates->getAll();
        $address = $this->_repoAddress->getFirstByUserId(intval($_SESSION["userId"]));
        //caso nom exista passamos a objeto vazia para podermos usar os methodos getWhatever...
        if (is_null($address))
            $address = new Address(null, null, null, null, null, null, null, null);

        return new Checkout($carrinho, $states, $address);
    }

    public function checkoutVerifyAuth()
    {
        if (is_null($_SESSION["userId"])) {
            return false;
        } else
            return true;
    }

    public function checkout()
    {
        $orderId = null;
        try {
            $cart = $_SESSION["cart"];

            //montando objeto de itens do pedido
            $orderItens = array();

            foreach ($cart->getProducts() as $product) {
                $orderItens[] = new OrderItem(
                    null,
                    null,
                    $product->getProductId(),
                    $product->getQtd()
                );
            }

            //montando objeto do pedido...
            $order = new Order(
                null,
                intval($_SESSION["userId"]),
                $cart->getTotalFinal(),
                $_POST["name"],
                $_POST["cardName"],
                $_POST["cardNumber"],
                $_POST["cardExpiration"],
                $_POST["cvv"],
                $_POST["cep"],
                $_POST["cpf"],
                $_POST["street"],
                $_POST["neighborhood"],
                $_POST["city"],
                $_POST["stateId"],
                $_POST["complement"],
                $orderItens
            );

            //persistindo os dados do pedido...
            $orderId = $this->_repoOrder->add($order);

            //se o pedido nao foi gerado sera lançado um erro
            if (is_null($orderId)) {
                throw new Exception();
            }

            //baixar estoque...
            $this->_repoProd->decreaseStockByOrderItens($orderItens);

            //limpando o carrinho...
            $_SESSION["cart"] = null;

            return new CheckoutResult(true, $orderId);
        } catch (Exception $ex) {
            //se ocorreram problemas removemos o pedido...
            if ($orderId != null)
                $this->_repoOrder->delete($orderId);

            return new CheckoutResult(false, null);
        }
    }
}
