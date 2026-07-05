<div class="address-container mb-3" data-id="<?php echo $address->getAddressId(); ?>">
                    
    <div class="row">
        <div class="col-md-4">
            <div class="form-group">
                <label for="">CEP:</label>
                <input type="text" data-required="true" class="form-control cep" 
                    name="cep" id="cep"  value="<?php echo $address->getCep();?>"  aria-describedby="cepId" placeholder="">
                <small id="cepId" class="form-text text-muted">
                    Seu cep.
                </small>
            </div>
        </div>
        <div class="col-md-8">
            <div class="form-group">
                <label for="">Rua:</label>
                <input type="text" data-required="true" class="form-control" 
                    name="street" id="street" value="<?php echo $address->getStreet();?>"
                    aria-describedby="street" placeholder="">
                <small id="streetId" class="form-text text-muted">
                    Sua rua, seu número de apto.
                </small>
            </div>
        </div>
        <div class="col-md-5">
            <div class="form-group">
                <label for="">Bairro:</label>
                <input type="text" data-required="true" value="<?php echo $address->getNeighborhood();?>" class="form-control" name="neighborhood" 
                        id="neighborhood"  aria-describedby="neighborhoodId" placeholder="">
                <small id="neighborhoodId" class="form-text text-muted">Seu bairro</small>
            </div>
        </div>
        <div class="col-md-3">
            <div class="form-group">
                <label for="">Estado:</label>
                <select data-required="true"  class="form-control" 
                    name="stateId" id="stateId" aria-describedby="stateIdhelp" placeholder="">
                    <option value="">Selecione</option>
                    <?php foreach($states as $state):?>
                        <option value="<?php echo $state->getId(); ?>" <?php echo ($address->getStateId() == $state->getId() ? "selected=selected" : ""); ?>>
                        <?php echo $state->getName(); ?>
                        </option>
                    <?php endforeach;?>
                </select>
                <small id="stateIdhelp" class="form-text text-muted">
                    Seu estado
                </small>
            </div>
        </div>
        <div class="col-md-4">
            <div class="form-group">
                <label for="">Cidade:</label>
                <input type="text" data-required="true" class="form-control" value="<?php echo $address->getCity();?>"
                    name="city" id="city"  aria-describedby="cityId" placeholder="">
                <small id="cityId" class="form-text text-muted">
                    Sua cidade
                </small>
            </div>
        </div>
        <div class="col-md-12">
            <div class="form-group">
                <label for="">Complemento:</label>
                <textarea data-required="true" class="form-control" 
                        id="complement" name="complement" 
                        aria-describedby="complement" placeholder=""><?php echo $address->getComplement();?></textarea>
                <small id="complementId" class="form-text text-muted">
                    Complemento
                </small>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-md-12 text-right">
            <button class="btn btn-danger btn-remover btn-sm" type="button" >
                <i class="fa fa-trash"></i> Remover endereço</button>
        </div>        
    </div>
</div>