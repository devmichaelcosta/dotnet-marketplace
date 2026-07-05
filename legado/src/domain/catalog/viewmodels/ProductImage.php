<?php

namespace domain\catalog\viewmodels;

use infra\helpers\SrcHelper;

class ProductImage
{
    private $image;

    public function __construct(string $image = null)
    {
        $this->image = SrcHelper::getProductImg() . $image;
    }

    /**
     * Get the value of image
     */
    public function getImage()
    {
        return $this->image;
    }

    /**
     * Set the value of image
     *
     * @return  self
     */
    public function setImage($image)
    {
        $this->image = $image;

        return $this;
    }
}
