<?php
    $qtdItensCarrinho = 0;
    if (isset($_SESSION["cart"])) {
        foreach ($_SESSION["cart"]->getProducts() as $productItem) {
            $qtdItensCarrinho += $productItem->getQtd();
        }
    }
?>
<div class=" container-nav">
    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <div class="row">
                    <div class="col-md-2">
                        <a class="navbar-brand" href="/">
                            <span class="logo-strong">L</span>oja<span class="logo-strong">W</span>hatever
                        </a>
                    </div>
                    <div class="col-md-8">
                        <form id="form-pesquisa" action="/pesquisa" method="GET">
                            <div class="row">
                                <div class="col-md-10">
                                    <div class="input-group mr-3">
                                        <input type="text" class="form-control" id="s" name="s" placeholder="Pesquisar produto" aria-label="Pesquisar produto" aria-describedby="button-addon2">
                                        <div class="input-group-append">
                                            <button class="btn btn-outline-light" type="submit" id="button-addon2">
                                                <i class="fa fa-search"></i>
                                            </button>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-2">
                                    <div class="row btn-group">
                                        <a href="/carrinho" class="btn btn-outline-light" data-toggle="tooltip" data-placement="body" title="Acessar seu carrinho" data-container="body">
                                            <i class="fa fa-cart-plus"></i>
                                            <span class="badge badge-light">
                                                <?php
                                                if (isset($_SESSION["cart"])) {
                                                    echo $_SESSION["cart"]->getTotalProdutos();
                                                } else {
                                                    echo "0";
                                                }
                                                ?>
                                            </span>
                                        </a>
                                        <?php if (isset($_SESSION["userId"])) : ?>
                                            <a href="/produto/curtidos" class="btn btn-outline-light" data-toggle="tooltip" data-placement="body" title="Acessar produtos curtidos" data-container="body">
                                                <i class="fa fa-heart"></i>
                                            </a>
                                        <?php endif; ?>
                                    </div>
                                </div>
                            </div>
                        </form>
                    </div>
                    <div class="col-md-2">
                        <span class="login-nav text-light">
                            <?php if (isset($_SESSION["userId"])) : ?>
                                <?php
                                if ($_SESSION["role"] == "vendedor") {
                                    $urlPerfil = "/admin/vendedor/editar?id=" . $_SESSION["sellerId"];
                                } else {
                                    $urlPerfil = "/admin/usuario/editar?id=" . $_SESSION["userId"];
                                }
                                ?>
                                <i class="fa fa-user"></i> Olá,
                                <a href="<?php echo $urlPerfil; ?>" class="a-primary">
                                    <?php echo $_SESSION["userName"] ?>
                                </a> clique em
                                <a href="/logout" class="a-primary" title="Sair no sistema" data-container="body" data-toggle="tooltip" data-placement="body">Sair
                                </a> para fazer seu logout.

                            <?php else : ?>
                                <i class="fa fa-user"></i> Olá , Faça seu
                                <a href="/login" class="a-primary" data-container="body" title="Entrar no sistema" data-toggle="tooltip" data-placement="body">Login</a>
                                <br /> ou
                                <a href="/registrar" class="a-primary" title="Entrar no sistema" data-container="body">Cadastre-se
                                </a>.
                            <?php endif; ?>
                        </span>
                    </div>
                </div>
            </div>
            <div class="col-md-12">
                <ul class="menu">
                    <li>
                        <a href="/">Home</a>
                    </li>
                    <li class=" dropdown">
                        <a class="dropdown-toggle" href="#" id="navbarDropdown" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                            Categorias
                        </a>
                        <?php
                            $factory = new infra\MySqlRepositoryFactory();
                            $categoriesMenu = $factory->getCategoryRepository()->getAll();
                        ?>
                        <div class="dropdown-menu" aria-labelledby="navbarDropdown">
                            <?php foreach ($categoriesMenu as $categoryMenuItem):?>
                                <a class="dropdown-item" href="/categorias?id=<?php echo $categoryMenuItem->getCategoryId(); ?>"><?php echo $categoryMenuItem->getTitle(); ?></a>
                            <?php endforeach;?>
                        </div>
                    </li>
                    <li>
                        <a href="/carrinho">Carrinho</a>
                    </li>

                    <?php if (!isset($_SESSION["role"])) : ?>
                        <li>
                            <a href="/vender">Vender</a>
                        </li>
                    <?php endif; ?>

                    <?php
                    
                        if (isset($_SESSION["userId"])) 
                        {
                            if ($_SESSION["role"] == "comum") {
                                require_once "./views/partials/nav-menu-default.php";
                            } else if ($_SESSION["role"] == "vendedor") {
                                require_once "./views/partials/nav-menu-seller.php";
                            } else {
                                require_once "./views/partials/nav-menu-admin.php";
                            }
                        }
                    ?>
                </ul>
            </div>
        </div>
    </div>
</div>